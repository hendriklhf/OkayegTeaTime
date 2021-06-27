using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Utils;
using Sterbehilfe.Strings;
using System;
using System.Collections.Generic;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Spotify
{
    public class LinkRecognizer
    {
        public string Message { get; set; }

        public LinkRecognizer(ChatMessage chatMessage)
        {
            Message = chatMessage.GetMessage();
        }

        public (bool, string) FindSpotifyLink()
        {
            if (Message.IsMatch(Pattern.SpotifyLinkPattern))
            {
                string uriCode = Message.Match(@"track/\w+\?").Remove("track/").Remove("?");
                return new(true, $"spotify:track:{uriCode}");
            }
            else
            {
                return new(false, string.Empty);
            }
        }
    }
}
