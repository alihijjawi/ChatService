using System;

namespace ChatService.DataContracts
{
    public class AddConversationResponse
    {
        public string Id { get; set; }
        public string[] Participants { get; set; }
        public DateTime LastModifiedDateUtc { get; set; }
    }
}
