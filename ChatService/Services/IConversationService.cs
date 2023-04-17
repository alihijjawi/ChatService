using ChatService.Dtos;

namespace ChatService.Services;

public interface IConversationService
{
    Task<StartConversationResponse> CreateConversation(string conversationId, ProfileDto senderProfile, long unixTime);

    Task<ConversationsList> GetConversationList(string username,
        string? continuationToken,
        string? limit,
        string? lastSeenMessageTime);

    Task<ConversationsList> GetConversationById(string conversationId);

    Task UpdateConversation(string conversationId, ProfileDto senderProfile, long unixTime);
}