using ChatService.Dtos;

namespace ChatService.Storage;

public interface IConversationsStore
{
    Task UpsertConversation(string conversationId, ProfileDto recipient, long unixTime);

    Task<ConversationsList> GetConversationList(string username, string? continuationToken, string? limit,
        string? lastSeenConversationTime);
    
    Task<ConversationsList> GetConversationById(string conversationId);
    
    Task DeleteConversation(string conversationId, string username);
}