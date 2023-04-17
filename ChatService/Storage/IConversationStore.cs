using ChatService.Dtos;

namespace ChatService.Storage;

public interface IConversationStore
{
    Task UpsertConversation(string conversationId, ProfileDto recipient, long unixTime);

    Task<ConversationsList> GetConversationList(string username, string? continuationToken, string? limit,
        string? lastSeenMessageTime);
    
    Task<ConversationsList> GetConversationById(string conversationId);
}