using System.Net;
using ChatService.Dtos;
using ChatService.Storage.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;

namespace ChatService.Storage;

public class CosmosConversationsStore : IConversationsStore
{
    private readonly CosmosClient _cosmosClient;

    public CosmosConversationsStore(CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
    }

    private Container ConversationsContainer =>
        _cosmosClient.GetDatabase("conversationsDb").GetContainer("conversations");

    public async Task StartConversation(string conversationId, ProfileDto recipient, long unixTime)
    {
        await ConversationsContainer.UpsertItemAsync(ToEntity(conversationId, recipient, unixTime));
    }

    public async Task<ConversationsList> GetConversationList(string username, string continuationToken, string limit,
        string lastSeenMessageTime)
    {
        var queryDefinition = new QueryDefinition("SELECT * FROM c");
        var requestOptions = new QueryRequestOptions
        {
            MaxItemCount = int.Parse(limit)
        };

        if (!string.IsNullOrEmpty(continuationToken))
        {
            requestOptions.ExecutionEnvironment = new Cosmos.Query.Core.ExecutionEnvironment
            {
                ContinuationToken = continuationToken
            };
        }

        var iterator = _container.GetItemQueryIterator<T>(queryDefinition, requestOptions: requestOptions);
        var response = await iterator.ReadNextAsync();
    }
    
    private static ConversationEntity ToEntity(string conversationId, ProfileDto recipient, long unixTime)
    {
        return new ConversationEntity(
            id: conversationId,
            Recipient: recipient,
            LastModifiedUnixTime: unixTime
        );
    }
}