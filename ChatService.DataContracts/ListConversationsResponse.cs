using System.Collections.Generic;

namespace ChatService.DataContracts
{
    public class ListConversationsResponse
    {
        public ListConversationsResponse(List<ListConversationsResponseItem> conversations, string nextUri)
        {
            Conversations = conversations;
            NextUri = nextUri;
        }

        public List<ListConversationsResponseItem> Conversations { get; }
        public string NextUri { get; }
    }
}
