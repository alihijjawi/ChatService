using ChatService.Dtos;

namespace ChatService.Services;

public interface IConversationsService
{
    Task EnqueueCreateConversation(ConversationDto conversation);
    
    Task<StartConversationResponse> CreateConversation(string conversationId, ProfileDto receiverProfile, long unixTime);

    Task<ConversationsList> GetConversationList(string username,
        string? continuationToken,
        string? limit,
        string? lastSeenConversationTime);

    Task<ConversationsList> GetConversationById(string conversationId);

    Task UpdateConversation(string conversationId, ProfileDto senderProfile, long unixTime);
    
    Task DeleteConversation(string conversationId, string username);
}