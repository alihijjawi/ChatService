using ChatService.Dtos;
using ChatService.Services.ServiceBus;
using ChatService.Storage;
using Microsoft.Azure.Cosmos.Linq;

namespace ChatService.Services;

public class ConversationsService : IConversationsService
{
    private readonly IConversationsStore _conversationsStore;
    private readonly ICreateConversationPublisher _createConversationPublisher;

    public ConversationsService(IConversationsStore conversationsStore, ICreateConversationPublisher createConversationPublisher)
    {
        _conversationsStore = conversationsStore;
        _createConversationPublisher = createConversationPublisher;
    }
    
    public async Task EnqueueCreateConversation(ConversationDto conversation)
    {
        await _createConversationPublisher.Send(conversation);
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