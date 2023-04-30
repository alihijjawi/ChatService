using System.Net;
using System.Web;
using ChatService.Dtos;
using ChatService.Storage.Entities;
using Microsoft.Azure.Cosmos;

namespace ChatService.Storage;

public class CosmosConversationsStore : IConversationsStore
{
    private readonly CosmosClient _cosmosClient;

    public CosmosConversationsStore(CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
    }

    private Container Container => _cosmosClient.GetDatabase("conversationsDb").GetContainer("conversations");

    public async Task UpsertConversation(string conversationId, ProfileDto recipient, long unixTime)
    {
        var entity = ToEntity(conversationId, recipient, unixTime);
        await Container.UpsertItemAsync(entity);
    }

    public async Task<ConversationsList?> GetConversationList(string username, string? continuationToken, string? limit,
        string? lastSeenConversationTime)
    {
        lastSeenConversationTime = lastSeenConversationTime ?? "0";
        
        limit = limit ?? "50";
        
        var queryDefinition = new QueryDefinition(
            "SELECT * FROM c " +
            $"WHERE c.LastModifiedUnixTime > {lastSeenConversationTime} " + 
            $"AND (c.id LIKE '{username}_%' OR c.id LIKE '%_{username}') " +
            $"AND c.Recipient.UserName != '{username}' " +
            "ORDER BY c.LastModifiedUnixTime DESC");

        var requestOptions = new QueryRequestOptions
        {
            MaxItemCount = int.Parse(limit)
        };

        var iterator = (continuationToken == null) ? 
            Container.GetItemQueryIterator<ConversationEntity>(queryDefinition, requestOptions: requestOptions) 
            : 
            Container.GetItemQueryIterator<ConversationEntity>(queryDefinition, requestOptions: requestOptions, continuationToken: continuationToken);
        
        var response = await iterator.ReadNextAsync();
            
        if (response.Diagnostics != null)
        {
            Console.WriteLine($"\nGetConversationList Diagnostics: {response.Diagnostics.ToString()}");
        }
        else
        {
            return null;
        }
            
        if (response.Count > 0)
        {
            continuationToken = response.ContinuationToken;
        }

        var conversationList = response.Select(ToConversation).ToArray();

        var nextUri = "";
        
        if (iterator.HasMoreResults)
        {
            nextUri = $"api/conversations?username={username}&limit={limit}&lastSeenMessageTime={lastSeenConversationTime}&continuationToken={HttpUtility.UrlEncode(continuationToken)}";
        }
        
        return new ConversationsList(conversationList,nextUri);
    }

    public async Task<ConversationsList> GetConversationById(string conversationId)
    {
        var queryDefinition = new QueryDefinition(
            "SELECT * FROM c " +
            $"WHERE c.id = '{conversationId}'");

        var iterator = Container.GetItemQueryIterator<ConversationEntity>(queryDefinition);
        
        var response = await iterator.ReadNextAsync();
            
        if (response.Diagnostics != null)
        {
            Console.WriteLine($"\nGetConversationById Diagnostics: {response.Diagnostics.ToString()}");
        }

        var conversationList = response.Select(ToConversation).ToArray();

        var nextUri = "";
        
        return new ConversationsList(conversationList,nextUri);
    }

    public async Task DeleteConversation(string conversationId, string username)
    {
        try
        {
            await Container.DeleteItemAsync<ConversationDto>(
                id: conversationId,
                partitionKey: new PartitionKey(username)
            );
        }
        catch (CosmosException e)
        {
            if (e.StatusCode == HttpStatusCode.NotFound)
            {
                return;
            }

            throw;
        }
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