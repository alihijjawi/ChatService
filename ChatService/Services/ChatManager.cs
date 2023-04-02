using ChatService.Dtos;
using Microsoft.Azure.Cosmos.Linq;

namespace ChatService.Services;

public class ChatManager: IChatManager
{
    private readonly IProfileService _profileService;
    private readonly IMessageService _messageService;
    private readonly IConversationService _conversationService;

    public ChatManager(IConversationService conversationStore, IProfileService profileService, IMessageService messageService)
    {
        _conversationService = conversationStore;
        _profileService = profileService;
        _messageService = messageService;
    }

    public async Task<StartConversationResponse> StartConversation(StartConversationRequest conversationRequest)
    {
        var senderProfile = await _profileService.GetProfile(conversationRequest.Participants[0]);
        var receiverProfile = await _profileService.GetProfile(conversationRequest.Participants[1]);

        if (senderProfile==null || receiverProfile==null) return null; //todo

        var senderConversationId = senderProfile.UserName + "_" + receiverProfile.UserName;
        var receiverConversationId = receiverProfile.UserName + "_" + senderProfile.UserName;

        var unixTime = DateTimeOffset.Now.ToUnixTimeSeconds();
        
        await _conversationService.CreateConversation(receiverConversationId, receiverProfile, unixTime);
        var conversationResponse =
            await _conversationService.CreateConversation(senderConversationId, senderProfile, unixTime);

        await _messageService.SendMessage(senderConversationId, conversationRequest.FirstMessage, unixTime);
        await _messageService.SendMessage(receiverConversationId, conversationRequest.FirstMessage, unixTime);

        return conversationResponse;
    }

    public async Task<ConversationsList> GetConversationList(string username, string? continuationToken, string? limit, string? lastSeenMessageTime)
    {
        return await _conversationService.GetConversationList(username, continuationToken, limit, lastSeenMessageTime);
    }

    public async Task<SendMessageResponse> SendMessage(string senderConversationId, SendMessageRequest messageRequest)
    {
        var unixTime = DateTimeOffset.Now.ToUnixTimeSeconds();

        var parts = senderConversationId.Split('_');
        Array.Reverse(parts);
        
        var receiverConversationId = string.Join('_', parts);

        await _messageService.SendMessage(receiverConversationId, messageRequest, unixTime);
        return await _messageService.SendMessage(senderConversationId, messageRequest, unixTime);
    }

    public async Task<MessagesList> GetMessageList(string conversationId, string? continuationToken, string? limit, string? lastSeenMessageTime)
    {
        return await _messageService.GetMessageList(conversationId, continuationToken, limit, lastSeenMessageTime);
    }
}