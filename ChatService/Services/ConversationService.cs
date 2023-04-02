using ChatService.Dtos;
using ChatService.Storage;
using Microsoft.Azure.Cosmos.Linq;

namespace ChatService.Services;

public class ConversationService : IConversationService
{
    private readonly IConversationStore _conversationStore;

    public ConversationService(IConversationStore conversationStore)
    {
        _conversationStore = conversationStore;
    }

    public async Task<StartConversationResponse> StartConversation(string conversationId, ProfileDto senderProfile)
    {
        var unixTime = await _conversationStore.CreateConversation(conversationId, senderProfile);
        return new StartConversationResponse(conversationId, unixTime);
    }

    public async Task<ConversationsList> GetConversationList(string username, string continuationToken, string limit,
        string lastSeenMessageTime)
    {
        return await _conversationStore.GetConversationList(username, continuationToken, limit, lastSeenMessageTime);
    }
}