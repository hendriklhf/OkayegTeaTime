using System;
using System.Text.RegularExpressions;
using HLE.Strings;
using OkayegTeaTime.Database;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Handlers;

public class WhisperHandler
{
    private static readonly Regex _spotifyCodeUrlPattern = new(Pattern.SpotifyCodeUrl, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    private static readonly Regex _codePattern = new(@"\?code=\S+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    public void Handle(TwitchWhisperMessage whisperMessage)
    {
        if (_spotifyCodeUrlPattern.IsMatch(whisperMessage.Message))
        {
            Match match = _codePattern.Match(whisperMessage.Message);
            string code = match.Value.Remove("?code=");
            (string AccessToken, string RefreshToken)? response = SpotifyController.GetNewAuthTokens(code).Result;
            if (response.HasValue)
            {
                DbControl.SpotifyUsers.Add(whisperMessage.Username, response.Value.AccessToken, response.Value.RefreshToken);
            }
        }
    }
}
