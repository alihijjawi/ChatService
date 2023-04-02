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

        if (senderProfile.IsNull() || receiverProfile.IsNull()) return null; //todo

        var senderId = senderProfile.UserName + "_" + receiverProfile.UserName;
        var receiverId = receiverProfile.UserName + "_" + senderProfile.UserName;

        var conversationResponse =
            await _conversationService.StartConversation(senderId, senderProfile);
        await _conversationService.StartConversation(receiverId, receiverProfile);

        await _messageService.SendMessage(senderId, conversationRequest.FirstMessage);
        await _messageService.SendMessage(receiverId, conversationRequest.FirstMessage);

        return conversationResponse;
    }

    public async Task<ConversationsList> GetConversationList(string username, string continuationToken, string limit, string lastSeenMessageTime)
    {
        return await _conversationService.GetConversationList(username, continuationToken, limit, lastSeenMessageTime);
    }

    public async Task<SendMessageResponse> SendMessage(string conversationId, SendMessageRequest messageRequest)
    {
        return await _messageService.SendMessage(conversationId, messageRequest);
    }

    public async Task<ConversationDto> GetMessageList(string conversationId, string continuationToken, string limit, string lastSeenMessageTime)
    {
        return await _messageService.GetMessageList(conversationId, continuationToken, limit, lastSeenMessageTime);
    }
}