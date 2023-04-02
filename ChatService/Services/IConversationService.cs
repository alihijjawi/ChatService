using ChatService.Dtos;

namespace ChatService.Services;

public interface IConversationService
{
    Task<StartConversationResponse> StartConversation(string conversationId, ProfileDto senderProfile);

    Task<ConversationsList> GetConversationList(string username,
        string continuationToken,
        string limit,
        string lastSeenMessageTime);
}