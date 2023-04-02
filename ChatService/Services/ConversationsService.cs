using ChatService.Dtos;
using ChatService.Storage;
using Microsoft.Azure.Cosmos.Linq;

namespace ChatService.Services;

public class ConversationsService : IConversationsService
{
    private readonly IProfileService _profileService;

    private readonly IMessagesService _messagesService;

    private readonly IConversationsStore _conversationsStore;

    public ConversationsService(IConversationsStore conversationsStore, IProfileService profileService, IMessagesService messagesService)
    {
        _conversationsStore = conversationsStore;
        _profileService = profileService;
        _messagesService = messagesService;
    }

    public async Task<StartConversationResponse> StartConversation(StartConversationRequest conversationRequest)
    {
        var senderProfile = await _profileService.GetProfile(conversationRequest.Participants[0]);
        var receiverProfile = await _profileService.GetProfile(conversationRequest.Participants[1]);

        if (senderProfile.IsNull() || receiverProfile.IsNull()) return null; //todo

        var unixTime = DateTimeOffset.Now.ToUnixTimeSeconds();

        var senderId = senderProfile.UserName + "_" + receiverProfile.UserName;
        var receiverId = receiverProfile.UserName + "_" + senderProfile.UserName;

        await _conversationsStore.StartConversation(senderId, senderProfile, unixTime);
        await _conversationsStore.StartConversation(receiverId, receiverProfile, unixTime);

        await _messagesService.SendMessage(senderId, conversationRequest.FirstMessage);
        await _messagesService.SendMessage(receiverId, conversationRequest.FirstMessage);

        return new StartConversationResponse(senderId, unixTime);
    }

    public Task<ConversationsList> GetConversationList(string username, string continuationToken, string limit,
        string lastSeenMessageTime)
    {
        return _conversationsStore.GetConversationList(username, continuationToken, limit, lastSeenMessageTime);
    }
}