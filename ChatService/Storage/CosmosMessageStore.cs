using ChatService.Dtos;
using ChatService.Storage.Entities;
using Microsoft.Azure.Cosmos;

namespace ChatService.Storage;

public class CosmosMessageStore: IMessageStore
{
    private readonly CosmosClient _cosmosClient;

    public CosmosMessageStore(CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
    }

    private Container Container => _cosmosClient.GetDatabase("conversationsDb").GetContainer("messages");
    
    public async Task<long> AddMessage(string conversationId, SendMessageRequest messageRequest)
    {
        var entity = ToEntity(messageRequest.MessageId, conversationId, messageRequest.SenderUsername, messageRequest.Text);
        await Container.UpsertItemAsync(entity);
        return entity.UnixTime;
    }

    public Task<ConversationDto> GetMessageList(string conversationId, string continuationToken, string limit, string lastSeenMessageTime)
    {
        //todo
        throw new NotImplementedException();
    }
    
    private static MessageEntity ToEntity(int messageId, string conversationId, string senderUsername, string text)
    {
        return new MessageEntity(
            id: messageId, 
            ConversationId: conversationId, 
            UnixTime: DateTimeOffset.Now.ToUnixTimeSeconds(), 
            SenderUsername: senderUsername,
            Text: text
        );
    }
}