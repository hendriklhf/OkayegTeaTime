using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using HLE.Collections;
using HLE.Memory;
using OkayegTeaTime.Twitch.Helix.Models;
using OkayegTeaTime.Twitch.Helix.Models.Responses;

namespace OkayegTeaTime.Twitch.Helix;

public sealed partial class TwitchApi
{
    public async ValueTask<User?> GetUserAsync(long userId)
    {
        if (TryGetUserFromCache(userId, out User? user))
        {
            return user;
        }

        using UrlBuilder urlBuilder = new(_apiBaseUrl, "users", _apiBaseUrl.Length + "users".Length + 50);
        urlBuilder.AppendParameter("id", userId);
        using HttpContentBytes response = await ExecuteRequestAsync(urlBuilder.ToString());
        GetResponse<User> getResponse = JsonSerializer.Deserialize<GetResponse<User>>(response.AsSpan());
        if (getResponse.Items.Length == 0)
        {
            return null;
        }

        user = getResponse.Items[0];
        Cache?.AddUser(user);
        return user;
    }

    public async ValueTask<User?> GetUserAsync(string username)
    {
        return await GetUserAsync(username.AsMemory());
    }

    public async ValueTask<User?> GetUserAsync(ReadOnlyMemory<char> username)
    {
        if (TryGetUserFromCache(username.Span, out User? user))
        {
            return user;
        }

        using UrlBuilder urlBuilder = new(_apiBaseUrl, "users", _apiBaseUrl.Length + "users".Length + 50);
        urlBuilder.AppendParameter("login", username.Span);
        using HttpContentBytes response = await ExecuteRequestAsync(urlBuilder.ToString());
        GetResponse<User> getResponse = JsonSerializer.Deserialize<GetResponse<User>>(response.AsSpan());
        if (getResponse.Items.Length == 0)
        {
            return null;
        }

        user = getResponse.Items[0];
        Cache?.AddUser(user);
        return user;
    }

    public async ValueTask<User[]> GetUsersAsync(IEnumerable<string> usernames)
    {
        // ReSharper disable once PossibleMultipleEnumeration
        if (usernames.TryGetReadOnlyMemory<string>(out ReadOnlyMemory<string> usernamesMemory))
        {
            return await GetUsersAsync(usernamesMemory, ReadOnlyMemory<long>.Empty);
        }

        // ReSharper disable once PossibleMultipleEnumeration
        return await GetUsersAsync(usernames.ToArray(), ReadOnlyMemory<long>.Empty);
    }

    public async ValueTask<User[]> GetUsersAsync(IEnumerable<long> userIds)
    {
        // ReSharper disable once PossibleMultipleEnumeration
        if (userIds.TryGetReadOnlyMemory(out var userIdsMemory))
        {
            return await GetUsersAsync(ReadOnlyMemory<string>.Empty, userIdsMemory);
        }

        // ReSharper disable once PossibleMultipleEnumeration
        return await GetUsersAsync(ReadOnlyMemory<string>.Empty, userIds.ToArray());
    }

    public async ValueTask<User[]> GetUsersAsync(IEnumerable<string> usernames, IEnumerable<long> userIds)
    {
        // ReSharper disable PossibleMultipleEnumeration
        bool usernamesIsMemory = usernames.TryGetReadOnlyMemory<string>(out ReadOnlyMemory<string> usernamesMemory);
        bool userIdsIsMemory = userIds.TryGetReadOnlyMemory<long>(out ReadOnlyMemory<long> userIdsMemory);

        return usernamesIsMemory switch
        {
            true when userIdsIsMemory => await GetUsersAsync(usernamesMemory, userIdsMemory),
            true when !userIdsIsMemory => await GetUsersAsync(usernamesMemory, userIds.ToArray()),
            false when userIdsIsMemory => await GetUsersAsync(usernamesMemory.ToArray(), userIdsMemory),
            _ => await GetUsersAsync(usernames.ToArray(), userIds.ToArray())
        };
        // ReSharper restore PossibleMultipleEnumeration
    }

    public async ValueTask<User[]> GetUsersAsync(List<string> usernames)
    {
        return await GetUsersAsync(CollectionsMarshal.AsSpan(usernames).AsMemoryUnsafe(), ReadOnlyMemory<long>.Empty);
    }

    public async ValueTask<User[]> GetUsersAsync(List<long> userIds)
    {
        return await GetUsersAsync(ReadOnlyMemory<string>.Empty, CollectionsMarshal.AsSpan(userIds).AsMemoryUnsafe());
    }

    public async ValueTask<User[]> GetUsersAsync(List<string> usernames, List<long> userIds)
    {
        return await GetUsersAsync(CollectionsMarshal.AsSpan(usernames).AsMemoryUnsafe(), CollectionsMarshal.AsSpan(userIds).AsMemoryUnsafe());
    }

    public async ValueTask<User[]> GetUsersAsync(params string[] usernames)
    {
        return await GetUsersAsync(usernames, ReadOnlyMemory<long>.Empty);
    }

    public async ValueTask<User[]> GetUsersAsync(params long[] userIds)
    {
        return await GetUsersAsync(ReadOnlyMemory<string>.Empty, userIds);
    }

    public async ValueTask<User[]> GetUsersAsync(string[] usernames, long[] userIds)
    {
        return await GetUsersAsync(usernames.AsMemory(), userIds);
    }

    public async ValueTask<User[]> GetUsersAsync(ReadOnlyMemory<string> usernames)
    {
        return await GetUsersAsync(usernames, ReadOnlyMemory<long>.Empty);
    }

    public async ValueTask<User[]> GetUsersAsync(ReadOnlyMemory<long> userIds)
    {
        return await GetUsersAsync(ReadOnlyMemory<string>.Empty, userIds);
    }

    public async ValueTask<User[]> GetUsersAsync(ReadOnlyMemory<string> usernames, ReadOnlyMemory<long> userIds)
    {
        using RentedArray<User> userBuffer = new(usernames.Length + userIds.Length);
        int userCount = await GetUsersAsync(usernames, userIds, userBuffer);
        return userCount == 0 ? Array.Empty<User>() : userBuffer[..userCount].ToArray();
    }

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

        using UrlBuilder urlBuilder = new(_apiBaseUrl, "users", usernames.Length * 35 + userIds.Length * 25 + 50);
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
        GetResponse<User> getResponse = JsonSerializer.Deserialize<GetResponse<User>>(response.AsSpan());
        int deserializedUserCount = getResponse.Items.Length;
        if (deserializedUserCount > 0)
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
