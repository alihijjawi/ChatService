using System;
using Newtonsoft.Json;

namespace ChatService.DataContracts
{
    public class ListMessagesResponseItem
    {
        public ListMessagesResponseItem(string text, string senderUsername, long unixTime)
        {
            Text = text;
            SenderUsername = senderUsername;
            UnixTime = unixTime;
        }

        public string Text { get; }
        public string SenderUsername { get; set; }
        public long UnixTime { get; }

        [JsonIgnore] public string LocalTime => DateTimeOffset.FromUnixTimeMilliseconds(UnixTime).LocalDateTime.ToShortTimeString();
    }
}