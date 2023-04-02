using ChatService.Dtos;

namespace ChatService.Services;

public interface IConversationsService
{
    Task<StartConversationResponse> StartConversation(StartConversationRequest conversationRequest);

    Task<ConversationsList> GetConversationList(string username,
        string continuationToken,
        string limit,
        string lastSeenMessageTime);
}