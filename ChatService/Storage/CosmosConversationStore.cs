using ChatService.Dtos;
using ChatService.Storage.Entities;
using Microsoft.Azure.Cosmos;

namespace ChatService.Storage;

public class CosmosConversationStore : IConversationStore
{
    private readonly CosmosClient _cosmosClient;

    public CosmosConversationStore(CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
    }

    private Container Container => _cosmosClient.GetDatabase("conversationsDb").GetContainer("conversations");

    public async Task<long> CreateConversation(string conversationId, ProfileDto recipient)
    {
        var entity = ToEntity(conversationId, recipient);
        await Container.UpsertItemAsync(entity);
        return entity.LastModifiedUnixTime;
    }

    public async Task<ConversationsList> GetConversationList(string username, string continuationToken, string limit,
        string lastSeenMessageTime)
    {
        // var queryDefinition = new QueryDefinition("SELECT * FROM c");
        // var requestOptions = new QueryRequestOptions
        // {
        //     MaxItemCount = int.Parse(limit)
        // };
        //
        // if (!string.IsNullOrEmpty(continuationToken))
        // {
        //     requestOptions.ExecutionEnvironment = new Cosmos.Query.Core.ExecutionEnvironment
        //     {
        //         ContinuationToken = continuationToken
        //     };
        // }
        //
        // var iterator = _container.GetItemQueryIterator<T>(queryDefinition, requestOptions: requestOptions);
        // var response = await iterator.ReadNextAsync();

        throw new NotImplementedException();
    }
    
    private static ConversationEntity ToEntity(string conversationId, ProfileDto recipient)
    {
        return new ConversationEntity(
            id: conversationId,
            Recipient: recipient,
            LastModifiedUnixTime: DateTimeOffset.Now.ToUnixTimeSeconds()
        );
    }
}