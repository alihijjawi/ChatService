using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Web;
using ChatService.Dtos;
using ChatService.Storage.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace ChatService.Storage;

public class CosmosConversationStore : IConversationStore
{
    private readonly CosmosClient _cosmosClient;

    public CosmosConversationStore(CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
    }

    private Container Container => _cosmosClient.GetDatabase("conversationsDb").GetContainer("conversations");

    public async Task CreateConversation(string conversationId, ProfileDto recipient, long unixTime)
    {
        var entity = ToEntity(conversationId, recipient, unixTime);
        await Container.UpsertItemAsync(entity);
    }

    public async Task<ConversationsList> GetConversationList(string username, string? continuationToken, string? limit,
        string? lastSeenMessageTime)
    {
        lastSeenMessageTime = lastSeenMessageTime ?? "0";
        
        limit = limit ?? "50";
        
        var queryDefinition = new QueryDefinition(
            "SELECT * FROM c " +
            $"WHERE c.LastModifiedUnixTime > {lastSeenMessageTime} " + 
            $"AND (c.id LIKE '{username}_%' OR c.id LIKE '%_{username}') " +
            $"AND c.Recipient.UserName = '{username}' " +
            "ORDER BY c.LastModifiedUnixTime");

        var requestOptions = new QueryRequestOptions
        {
            MaxItemCount = int.Parse(limit)
        };

        var iterator = continuationToken == null ? 
            Container.GetItemQueryIterator<ConversationEntity>(queryDefinition, requestOptions: requestOptions) 
            : 
            Container.GetItemQueryIterator<ConversationEntity>(queryDefinition, requestOptions: requestOptions, continuationToken: continuationToken);
        
        var response = await iterator.ReadNextAsync();
            
        if (response.Diagnostics != null)
        {
            Console.WriteLine($"\nGetConversationList Diagnostics: {response.Diagnostics.ToString()}");
        }
            
        if (response.Count > 0)
        {
            continuationToken = response.ContinuationToken;
        }

        var conversationList = response.Select(ToConversation).ToArray();

        var nextUrl = $"/conversations?username={username}&limit={limit}&lastSeenMessageTime={lastSeenMessageTime}&continuationToken={HttpUtility.UrlEncode(continuationToken)}";
        
        return new ConversationsList(conversationList,nextUrl);
    }
    
    private static ConversationEntity ToEntity(string conversationId, ProfileDto recipient, long unixTime)
    {
        return new ConversationEntity(
            id: conversationId,
            Recipient: recipient,
            LastModifiedUnixTime: unixTime
        );
    }
    
    private static ConversationDto ToConversation(ConversationEntity conversationEntity)
    {
        return new ConversationDto(
            Id: conversationEntity.id,
            Recipient: conversationEntity.Recipient,
            LastModifiedUnixTime: conversationEntity.LastModifiedUnixTime
        );
    }
}