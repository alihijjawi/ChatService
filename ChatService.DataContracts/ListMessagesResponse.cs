using System.Collections.Generic;
using System.Linq;

namespace ChatService.DataContracts
{
    public class ListMessagesResponse
    {
        public string NextUri { get; }

        public ListMessagesResponse(IEnumerable<ListMessagesResponseItem> messages, string nextUri)
        {
            NextUri = nextUri;
            Messages = messages.ToList();
        }

        public List<ListMessagesResponseItem> Messages { get; }
    }
}
