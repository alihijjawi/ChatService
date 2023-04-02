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

    public async Task<StartConversationResponse> CreateConversation(string conversationId, ProfileDto senderProfile, long unixTime)
    {
        await _conversationStore.CreateConversation(conversationId, senderProfile, unixTime);
        return new StartConversationResponse(conversationId, unixTime);
    }

    public async Task<ConversationsList> GetConversationList(string username, string? continuationToken, string? limit,
        string? lastSeenMessageTime)
    {
        return await _conversationStore.GetConversationList(username, continuationToken, limit, lastSeenMessageTime);
    }
}