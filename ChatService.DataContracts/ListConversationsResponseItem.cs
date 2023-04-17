using System;

namespace ChatService.DataContracts
{
    public class ListConversationsResponseItem
    {
        public ListConversationsResponseItem(string id, UserProfileDto recipient, long lastModifiedUnixTime)
        {
            Id = id;
            Recipient = recipient;
            LastModifiedUnixTime = lastModifiedUnixTime;
        }

        public string Id { get; }
        public UserProfileDto Recipient { get; }
        public long LastModifiedUnixTime { get; }
    }
}
