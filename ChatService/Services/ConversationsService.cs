using ChatService.Dtos;

namespace ChatService.Services;

public class ConversationsService: IConversationsService
{
    public Task<StartConversationResponse> StartConversation()
    {
        throw new NotImplementedException();
    }

    public Task<SendMessageResponse> SendMessage()
    {
        throw new NotImplementedException();
    }

    public Task<ConversationDto> GetConversation()
    {
        throw new NotImplementedException();
    }

    public Task<ConversationsList> GetConversationList()
    {
        throw new NotImplementedException();
    }
}