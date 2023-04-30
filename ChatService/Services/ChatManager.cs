using System.Data;
using ChatService.Dtos;
using Microsoft.Azure.Cosmos.Linq;

namespace ChatService.Services;

public class ChatManager : IChatManager
{
    private readonly IProfileService _profileService;
    private readonly IMessageService _messageService;
    private readonly IConversationsService _conversationsService;

    public ChatManager(IConversationsService conversationsStore, IProfileService profileService,
        IMessageService messageService)
    {
        _conversationsService = conversationsStore;
        _profileService = profileService;
        _messageService = messageService;
    }

    public async Task<StartConversationResponse?> StartConversation(StartConversationRequest conversationRequest)
    {
        var senderProfile = await _profileService.GetProfile(conversationRequest.Participants[0]);
        var receiverProfile = await _profileService.GetProfile(conversationRequest.Participants[1]);

        if (senderProfile == null || receiverProfile == null) throw new DataException();

        var conversationId =
            (String.Compare(senderProfile.UserName,
                receiverProfile.UserName,
                comparisonType: StringComparison.OrdinalIgnoreCase) > 0)
                ? senderProfile.UserName + "_" + receiverProfile.UserName
                : receiverProfile.UserName + "_" + senderProfile.UserName;

        var unixTime = DateTimeOffset.Now.ToUnixTimeSeconds();

        await _conversationsService.CreateConversation(conversationId, senderProfile, unixTime);
        var conversationResponse = await _conversationsService.CreateConversation(conversationId, receiverProfile, unixTime);
        
        await _messageService.SendMessage(conversationId, conversationRequest.FirstMessage, unixTime);

        return conversationResponse;
    }

    public async Task<ConversationsList?> GetConversationList(string username, string? continuationToken, string? limit,
        string? lastSeenConversationTime)
    {
        var profile = await _profileService.GetProfile(username);
        if (profile == null) throw new DataException();
        return await _conversationsService.GetConversationList(username, continuationToken, limit, lastSeenConversationTime);
    }

    public async Task<SendMessageResponse> SendMessage(string conversationId, SendMessageRequest messageRequest)
    {
        var unixTime = DateTimeOffset.Now.ToUnixTimeSeconds();

        var conversations = await _conversationsService.GetConversationById(conversationId);

        var response = await _messageService.SendMessage(conversationId, messageRequest, unixTime);

        var recipient1 = conversations.Conversations[0].Recipient;
        
        var recipient2 = conversations.Conversations[1].Recipient;
        
        await Task.WhenAll(
            _conversationsService.UpdateConversation(conversationId, recipient1, unixTime),
            _conversationsService.UpdateConversation(conversationId, recipient2, unixTime)
        );
        
        return response;
    }

    public async Task<MessagesList?> GetMessageList(string conversationId, string? continuationToken, string? limit,
        string? lastSeenMessageTime)
    {
        return await _messageService.GetMessageList(conversationId, continuationToken, limit, lastSeenMessageTime);
    }
}