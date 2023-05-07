using Newtonsoft.Json;

namespace ChatService.DataContracts
{
    public class AddConversationRequest
    {
        public AddConversationRequest()
        {
        }
        
        [JsonConstructor]
        public AddConversationRequest(SendMessageRequest firstMessage, params string[] participants)
        {
            FirstMessage = firstMessage;
            Participants = participants;
        }
        public string[] Participants { get; set; }
        public SendMessageRequest FirstMessage { get; set; }
    }
}
