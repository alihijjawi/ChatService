using ChatService.Dtos;

namespace ChatService.Storage;

public interface IConversationsStore
{
    Task StartConversation(string conversationId, ProfileDto recipient, long unixTime);

    Task<ConversationsList> GetConversationList(string username, string continuationToken, string limit,
        string lastSeenMessageTime);
}