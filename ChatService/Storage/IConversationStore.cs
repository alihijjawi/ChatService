using ChatService.Dtos;

namespace ChatService.Storage;

public interface IConversationStore
{
    Task<long> CreateConversation(string conversationId, ProfileDto recipient);

    Task<ConversationsList> GetConversationList(string username, string continuationToken, string limit,
        string lastSeenMessageTime);
}