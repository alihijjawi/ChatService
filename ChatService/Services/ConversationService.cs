using ChatService.Dtos;
using ChatService.Storage;
using Microsoft.Azure.Cosmos.Linq;

namespace ChatService.Services;

public class ConversationsService : IConversationsService
{
    private readonly IConversationsStore _conversationsStore;

    public ConversationsService(IConversationsStore conversationsStore)
    {
        _conversationsStore = conversationsStore;
    }

    public async Task<StartConversationResponse> CreateConversation(string conversationId, ProfileDto receiverProfile, long unixTime)
    {
        await _conversationsStore.CreateConversation(conversationId, receiverProfile, unixTime);
        return new StartConversationResponse(conversationId, unixTime);
    }

    public async Task UpdateConversation(string conversationId, ProfileDto senderProfile, long unixTime)
    {
        await _conversationsStore.UpsertConversation(conversationId, senderProfile, unixTime);
    }

    public async Task<ConversationsList> GetConversationById(string conversationId)
    {
        return await _conversationsStore.GetConversationById(conversationId);
    }

    public async Task<ConversationsList?> GetConversationList(string username, string? continuationToken, string? limit,
        string? lastSeenConversationTime)
    {
        return await _conversationsStore.GetConversationList(username, continuationToken, limit, lastSeenConversationTime);
    }

    public async Task DeleteConversation(string conversationId, string username)
    {
        await _conversationsStore.DeleteConversation(conversationId, username);
    }
}