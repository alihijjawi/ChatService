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
        await _conversationStore.UpsertConversation(conversationId, senderProfile, unixTime);
        return new StartConversationResponse(conversationId, unixTime);
    }

    public async Task UpdateConversation(string conversationId, ProfileDto senderProfile, long unixTime)
    {
        await _conversationStore.UpsertConversation(conversationId, senderProfile, unixTime);
    }

    public async Task<ConversationsList> GetConversationById(string conversationId)
    {
        return await _conversationStore.GetConversationById(conversationId);
    }

    public async Task<ConversationsList?> GetConversationList(string username, string? continuationToken, string? limit,
        string? lastSeenConversationTime)
    {
        return await _conversationStore.GetConversationList(username, continuationToken, limit, lastSeenConversationTime);
    }
}