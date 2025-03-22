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
    private const string UsersEndpoint = "users";

    public async ValueTask<User?> GetUserAsync(long userId)
    {
        if (TryGetUserFromCache(userId, out User? user))
        {
            return user;
        }

        using UrlBuilder urlBuilder = new(ApiBaseUrl, UsersEndpoint, ApiBaseUrl.Length + UsersEndpoint.Length + 50);
        urlBuilder.AppendParameter("id", userId);
        using HttpContentBytes response = await ExecuteRequestAsync(urlBuilder.ToString());
        GetResponse<User> getResponse = JsonSerializer.Deserialize(response.AsSpan(), HelixJsonSerializerContext.Default.GetResponseUser);
        if (getResponse.Items.Length == 0)
        {
            return null;
        }

        user = getResponse.Items[0];
        Cache?.AddUser(user);
        return user;
    }

    public ValueTask<User?> GetUserAsync(string username) => GetUserAsync(username.AsMemory());

    public async ValueTask<User?> GetUserAsync(ReadOnlyMemory<char> username)
    {
        if (TryGetUserFromCache(username.Span, out User? user))
        {
            return user;
        }

        using UrlBuilder urlBuilder = new(ApiBaseUrl, UsersEndpoint, ApiBaseUrl.Length + UsersEndpoint.Length + 50);
        urlBuilder.AppendParameter("login", username.Span);
        using HttpContentBytes response = await ExecuteRequestAsync(urlBuilder.ToString());
        GetResponse<User> getResponse = JsonSerializer.Deserialize(response.AsSpan(), HelixJsonSerializerContext.Default.GetResponseUser);
        if (getResponse.Items.Length == 0)
        {
            return null;
        }

        user = getResponse.Items[0];
        Cache?.AddUser(user);
        return user;
    }

    public ValueTask<User[]> GetUsersAsync(IEnumerable<string> usernames)
    {
        // ReSharper disable once PossibleMultipleEnumeration
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (usernames.TryGetReadOnlyMemory(out ReadOnlyMemory<string> usernamesMemory))
        {
            return GetUsersAsync(usernamesMemory, ReadOnlyMemory<long>.Empty);
        }

        // ReSharper disable once PossibleMultipleEnumeration
        return GetUsersAsync(usernames.ToArray(), ReadOnlyMemory<long>.Empty);
    }

    public ValueTask<User[]> GetUsersAsync(IEnumerable<long> userIds)
    {
        // ReSharper disable once PossibleMultipleEnumeration
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (userIds.TryGetReadOnlyMemory(out ReadOnlyMemory<long> userIdsMemory))
        {
            return GetUsersAsync(ReadOnlyMemory<string>.Empty, userIdsMemory);
        }

        // ReSharper disable once PossibleMultipleEnumeration
        return GetUsersAsync(ReadOnlyMemory<string>.Empty, userIds.ToArray());
    }

    public async ValueTask<User[]> GetUsersAsync(IEnumerable<string> usernames, IEnumerable<long> userIds)
    {
        // ReSharper disable PossibleMultipleEnumeration
        bool usernamesIsMemory = usernames.TryGetReadOnlyMemory(out ReadOnlyMemory<string> usernamesMemory);
        bool userIdsIsMemory = userIds.TryGetReadOnlyMemory(out ReadOnlyMemory<long> userIdsMemory);

        return usernamesIsMemory switch
        {
            true when userIdsIsMemory => await GetUsersAsync(usernamesMemory, userIdsMemory),
            true when !userIdsIsMemory => await GetUsersAsync(usernamesMemory, userIds.ToArray()),
            false when userIdsIsMemory => await GetUsersAsync(usernamesMemory.ToArray(), userIdsMemory),
            _ => await GetUsersAsync(usernames.ToArray(), userIds.ToArray())
        };
        // ReSharper restore PossibleMultipleEnumeration
    }

    public ValueTask<User[]> GetUsersAsync(List<string> usernames)
        => GetUsersAsync(SpanMarshal.AsMemory(CollectionsMarshal.AsSpan(usernames)), ReadOnlyMemory<long>.Empty);

    public ValueTask<User[]> GetUsersAsync(List<long> userIds)
        => GetUsersAsync(ReadOnlyMemory<string>.Empty, SpanMarshal.AsMemory(CollectionsMarshal.AsSpan(userIds)));

    public ValueTask<User[]> GetUsersAsync(List<string> usernames, List<long> userIds)
        => GetUsersAsync(SpanMarshal.AsMemory(CollectionsMarshal.AsSpan(usernames)), SpanMarshal.AsMemory(CollectionsMarshal.AsSpan(userIds)));

    public ValueTask<User[]> GetUsersAsync(params string[] usernames)
        => GetUsersAsync(usernames, ReadOnlyMemory<long>.Empty);

    public ValueTask<User[]> GetUsersAsync(params long[] userIds)
        => GetUsersAsync(ReadOnlyMemory<string>.Empty, userIds);

    public ValueTask<User[]> GetUsersAsync(string[] usernames, long[] userIds)
        => GetUsersAsync(usernames.AsMemory(), userIds);

    public ValueTask<User[]> GetUsersAsync(ReadOnlyMemory<string> usernames)
        => GetUsersAsync(usernames, ReadOnlyMemory<long>.Empty);

    public ValueTask<User[]> GetUsersAsync(ReadOnlyMemory<long> userIds)
        => GetUsersAsync(ReadOnlyMemory<string>.Empty, userIds);

    public async ValueTask<User[]> GetUsersAsync(ReadOnlyMemory<string> usernames, ReadOnlyMemory<long> userIds)
    {
        User[] userBuffer = ArrayPool<User>.Shared.Rent(usernames.Length + userIds.Length);
        int userCount = await GetUsersAsync(usernames, userIds, userBuffer.AsMemory());
        User[] users = userCount == 0 ? [] : userBuffer[..userCount].ToArray();
        ArrayPool<User>.Shared.Return(userBuffer);
        return users;
    }

    [SuppressMessage("Major Code Smell", "S4457:Parameter validation in \"async\"/\"await\" methods should be wrapped")]
    public async ValueTask<int> GetUsersAsync(ReadOnlyMemory<string> usernames, ReadOnlyMemory<long> userIds, Memory<User> resultBuffer)
    {
        int parameterCount = usernames.Length + userIds.Length;
        switch (parameterCount)
        {
            case 0:
                return 0;
            case > 100:
                throw new ArgumentException("The endpoint allows only up to 100 parameters. You can't pass more than 100 usernames or user ids in total.");
        }

        using UrlBuilder urlBuilder = new(ApiBaseUrl, UsersEndpoint, usernames.Length * 35 + userIds.Length * 25 + 50);
        int cachedUserCount = 0;
        for (int i = 0; i < usernames.Length; i++)
        {
            string username = usernames.Span[i];
            if (TryGetUserFromCache(username, out User? user))
            {
                resultBuffer.Span[cachedUserCount++] = user;
                continue;
            }

            urlBuilder.AppendParameter("login", username);
        }

        for (int i = 0; i < userIds.Length; i++)
        {
            long userId = userIds.Span[i];
            if (TryGetUserFromCache(userId, out User? user))
            {
                resultBuffer.Span[cachedUserCount++] = user;
                continue;
            }

            urlBuilder.AppendParameter("id", userId);
        }

        if (urlBuilder.ParameterCount == 0)
        {
            return cachedUserCount;
        }

        using HttpContentBytes response = await ExecuteRequestAsync(urlBuilder.ToString());
        GetResponse<User> getResponse = JsonSerializer.Deserialize(response.AsSpan(), HelixJsonSerializerContext.Default.GetResponseUser);
        int deserializedUserCount = getResponse.Items.Length;
        if (deserializedUserCount != 0)
        {
            getResponse.Items.CopyTo(resultBuffer.Span[cachedUserCount..]);
        }

        Cache?.AddUsers(resultBuffer.Span[cachedUserCount..(cachedUserCount + deserializedUserCount)]);
        return deserializedUserCount + cachedUserCount;
    }

    private bool TryGetUserFromCache(long userId, [MaybeNullWhen(false)] out User user)
    {
        user = null;
        return Cache?.TryGetUser(userId, out user) == true;
    }

    private bool TryGetUserFromCache(ReadOnlySpan<char> username, [MaybeNullWhen(false)] out User user)
    {
        user = null;
        return Cache?.TryGetUser(username, out user) == true;
    }
}
