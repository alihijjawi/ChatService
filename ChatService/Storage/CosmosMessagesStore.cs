using ChatService.Dtos;

namespace ChatService.Storage;

public class CosmosMessagesStore: IMessagesStore
{
    public Task<SendMessageResponse> SendMessage(string conversationId, SendMessageRequest messageRequest)
    {
        throw new NotImplementedException();
    }

    public Task<ConversationDto> GetMessageList(string conversationId, string continuationToken, string limit, string lastSeenMessageTime)
    {
        throw new NotImplementedException();
    }
}