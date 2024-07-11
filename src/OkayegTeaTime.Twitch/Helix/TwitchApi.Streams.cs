using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using HLE.Collections;
using HLE.Marshalling;
using HLE.Memory;
using OkayegTeaTime.Twitch.Helix.Models;
using OkayegTeaTime.Twitch.Helix.Models.Responses;

namespace OkayegTeaTime.Twitch.Helix;

public sealed partial class TwitchApi
{
    private const string StreamsEndpointName = "streams";

    public async ValueTask<Stream?> GetStreamAsync(long userId)
    {
        if (TryGetStreamFromCache(userId, out Stream? stream))
        {
            return stream;
        }

        using UrlBuilder urlBuilder = new(ApiBaseUrl, StreamsEndpointName, ApiBaseUrl.Length + StreamsEndpointName.Length + 50);
        urlBuilder.AppendParameter("user_id", userId);
        using HttpContentBytes response = await ExecuteRequestAsync(urlBuilder.ToString());
        GetResponse<Stream> getResponse = JsonSerializer.Deserialize(response.AsSpan(), HelixJsonSerializerContext.Default.GetResponseStream);
        if (getResponse.Items.Length == 0)
        {
            return null;
        }

        stream = getResponse.Items[0];
        Cache?.AddStream(stream);
        return stream;
    }

    public async ValueTask<Stream?> GetStreamAsync(string username) => await GetStreamAsync(username.AsMemory());

    public async ValueTask<Stream?> GetStreamAsync(ReadOnlyMemory<char> username)
    {
        if (TryGetStreamFromCache(username.Span, out Stream? stream))
        {
            return stream;
        }

        using UrlBuilder urlBuilder = new(ApiBaseUrl, StreamsEndpointName, ApiBaseUrl.Length + StreamsEndpointName.Length + 50);
        urlBuilder.AppendParameter("user_login", username.Span);
        using HttpContentBytes response = await ExecuteRequestAsync(urlBuilder.ToString());
        GetResponse<Stream> getResponse = JsonSerializer.Deserialize(response.AsSpan(), HelixJsonSerializerContext.Default.GetResponseStream);
        if (getResponse.Items.Length == 0)
        {
            return null;
        }

        stream = getResponse.Items[0];
        Cache?.AddStream(stream);
        return stream;
    }

    public async ValueTask<Stream[]> GetStreamsAsync(IEnumerable<string> usernames)
    {
        // ReSharper disable once PossibleMultipleEnumeration
        if (usernames.TryGetReadOnlyMemory<string>(out ReadOnlyMemory<string> usernamesMemory))
        {
            return await GetStreamsAsync(usernamesMemory, ReadOnlyMemory<long>.Empty);
        }

        // ReSharper disable once PossibleMultipleEnumeration
        return await GetStreamsAsync(usernames.ToArray(), ReadOnlyMemory<long>.Empty);
    }

    public async ValueTask<Stream[]> GetStreamsAsync(IEnumerable<long> channelIds)
    {
        // ReSharper disable once PossibleMultipleEnumeration
        if (channelIds.TryGetReadOnlyMemory(out ReadOnlyMemory<long> channelIdsMemory))
        {
            return await GetStreamsAsync(ReadOnlyMemory<string>.Empty, channelIdsMemory);
        }

        // ReSharper disable once PossibleMultipleEnumeration
        return await GetStreamsAsync(ReadOnlyMemory<string>.Empty, channelIds.ToArray());
    }

    public async ValueTask<Stream[]> GetStreamsAsync(IEnumerable<string> usernames, IEnumerable<long> channelIds)
    {
        // ReSharper disable PossibleMultipleEnumeration
        bool usernamesIsMemory = usernames.TryGetReadOnlyMemory<string>(out ReadOnlyMemory<string> usernamesMemory);
        bool channelIdsIsMemory = channelIds.TryGetReadOnlyMemory(out ReadOnlyMemory<long> channelIdsMemory);

        return usernamesIsMemory switch
        {
            true when channelIdsIsMemory => await GetStreamsAsync(usernamesMemory, channelIdsMemory),
            true when !channelIdsIsMemory => await GetStreamsAsync(usernamesMemory, channelIds.ToArray()),
            false when channelIdsIsMemory => await GetStreamsAsync(usernamesMemory.ToArray(), channelIdsMemory),
            _ => await GetStreamsAsync(usernames.ToArray(), channelIds.ToArray())
        };
        // ReSharper restore PossibleMultipleEnumeration
    }

    public async ValueTask<Stream[]> GetStreamsAsync(List<string> usernames)
        => await GetStreamsAsync(SpanMarshal.AsMemory(CollectionsMarshal.AsSpan(usernames)), ReadOnlyMemory<long>.Empty);

    public async ValueTask<Stream[]> GetStreamsAsync(List<long> channelIds)
        => await GetStreamsAsync(ReadOnlyMemory<string>.Empty, SpanMarshal.AsMemory(CollectionsMarshal.AsSpan(channelIds)));

    public async ValueTask<Stream[]> GetStreamsAsync(List<string> usernames, List<long> channelIds)
        => await GetStreamsAsync(SpanMarshal.AsMemory(CollectionsMarshal.AsSpan(usernames)), SpanMarshal.AsMemory(CollectionsMarshal.AsSpan(channelIds)));

    public async ValueTask<Stream[]> GetStreamsAsync(params string[] usernames)
        => await GetStreamsAsync(usernames, ReadOnlyMemory<long>.Empty);

    public async ValueTask<Stream[]> GetStreamsAsync(params long[] channelIds)
        => await GetStreamsAsync(ReadOnlyMemory<string>.Empty, channelIds);

    public async ValueTask<Stream[]> GetStreamsAsync(string[] usernames, long[] channelIds)
        => await GetStreamsAsync(usernames.AsMemory(), channelIds);

    public async ValueTask<Stream[]> GetStreamsAsync(ReadOnlyMemory<string> usernames)
        => await GetStreamsAsync(usernames, ReadOnlyMemory<long>.Empty);

    public async ValueTask<Stream[]> GetStreamsAsync(ReadOnlyMemory<long> channelIds)
        => await GetStreamsAsync(ReadOnlyMemory<string>.Empty, channelIds);

    public async ValueTask<Stream[]> GetStreamsAsync(ReadOnlyMemory<string> usernames, ReadOnlyMemory<long> channelIds)
    {
        using RentedArray<Stream> streamBuffer = ArrayPool<Stream>.Shared.RentAsRentedArray(usernames.Length + channelIds.Length);
        int streamCount = await GetStreamsAsync(usernames, channelIds, streamBuffer.AsMemory());
        return streamCount == 0 ? [] : streamBuffer[..streamCount].ToArray();
    }

    [SuppressMessage("Major Code Smell", "S4457:Parameter validation in \"async\"/\"await\" methods should be wrapped")]
    public async ValueTask<int> GetStreamsAsync(ReadOnlyMemory<string> usernames, ReadOnlyMemory<long> channelIds, Memory<Stream> resultBuffer)
    {
        int parameterCount = usernames.Length + channelIds.Length;
        switch (parameterCount)
        {
            case 0:
                return 0;
            case 100:
                throw new ArgumentException("The endpoint allows only up to 100 parameters. You can't pass more than 100 usernames or user ids in total.");
        }

        using UrlBuilder urlBuilder = new(ApiBaseUrl, StreamsEndpointName, usernames.Length * 35 + channelIds.Length * 25 + 50);
        int cachedStreamCount = 0;
        for (int i = 0; i < usernames.Length; i++)
        {
            string username = usernames.Span[i];
            if (TryGetStreamFromCache(username, out Stream? stream))
            {
                resultBuffer.Span[cachedStreamCount++] = stream;
                continue;
            }

            urlBuilder.AppendParameter("user_login", username);
        }

        for (int i = 0; i < channelIds.Length; i++)
        {
            long userId = channelIds.Span[i];
            if (TryGetStreamFromCache(userId, out Stream? stream))
            {
                resultBuffer.Span[cachedStreamCount++] = stream;
                continue;
            }

            urlBuilder.AppendParameter("user_id", userId);
        }

        if (urlBuilder.ParameterCount == 0)
        {
            return cachedStreamCount;
        }

        using HttpContentBytes response = await ExecuteRequestAsync(urlBuilder.ToString());
        GetResponse<Stream> getResponse = JsonSerializer.Deserialize(response.AsSpan(), HelixJsonSerializerContext.Default.GetResponseStream);
        int deserializedStreamCount = getResponse.Items.Length;
        if (deserializedStreamCount != 0)
        {
            getResponse.Items.CopyTo(resultBuffer.Span[cachedStreamCount..]);
        }

        Cache?.AddStreams(resultBuffer.Span[cachedStreamCount..(cachedStreamCount + deserializedStreamCount)]);
        return deserializedStreamCount + cachedStreamCount;
    }

    private bool TryGetStreamFromCache(long channelId, [MaybeNullWhen(false)] out Stream stream)
    {
        stream = null;
        return Cache?.TryGetStream(channelId, out stream) == true;
    }

    private bool TryGetStreamFromCache(ReadOnlySpan<char> username, [MaybeNullWhen(false)] out Stream stream)
    {
        stream = null;
        return Cache?.TryGetStream(username, out stream) == true;
    }
}
