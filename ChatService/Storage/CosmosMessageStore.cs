﻿using System.Data;
using System.Net;
using System.Web;
using ChatService.Dtos;
using ChatService.Storage.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace ChatService.Storage;

public class CosmosMessageStore: IMessageStore
{
    private readonly CosmosClient _cosmosClient;

    public CosmosMessageStore(CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
    }

    private Container Container => _cosmosClient.GetDatabase("conversationsDb").GetContainer("messages");
    
    public async Task AddMessage(string conversationId, SendMessageRequest messageRequest, long unixTime)
    {
        var entity = ToEntity(messageRequest.Id, conversationId, messageRequest.SenderUsername, messageRequest.Text, unixTime);
        
        var response = await Container.CreateItemAsync(entity);

        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            throw new Exception("Duplicate message id sent.");
        }
    }

    public async Task<MessagesList?> GetMessageList(string conversationId, string? continuationToken, string? limit, string? lastSeenMessageTime)
    {
        lastSeenMessageTime = lastSeenMessageTime ?? "0";
        
        limit = limit ?? "50";
        
        var queryDefinition = new QueryDefinition(
            "SELECT * FROM c " +
            $"WHERE c.UnixTime > {lastSeenMessageTime} " + 
            $"AND c.ConversationId = '{conversationId}' " +
            "ORDER BY c.UnixTime DESC");

        var requestOptions = new QueryRequestOptions
        {
            MaxItemCount = int.Parse(limit),
        };

        var iterator = (continuationToken == null) ? 
            Container.GetItemQueryIterator<MessageEntity>(queryDefinition, requestOptions: requestOptions) 
            : 
            Container.GetItemQueryIterator<MessageEntity>(queryDefinition, requestOptions: requestOptions, continuationToken: continuationToken);

        var response = await iterator.ReadNextAsync();
        
        if (response.Count == 0)
        {
            throw new DataException();
        }
            
        if (response.Diagnostics != null)
        {
            Console.WriteLine($"\nGetMessageList Diagnostics: {response.Diagnostics.ToString()}");
        }
        else
        {
            return null;
        }
         
        continuationToken = response.ContinuationToken;

        var conversationList = response.Select(ToMessage).ToArray();
        
        var nextUri = "";

        if (iterator.HasMoreResults)
        {
            nextUri =
                $"api/conversations/{conversationId}/messages?&limit={limit}&lastSeenMessageTime={lastSeenMessageTime}&continuationToken={HttpUtility.UrlEncode(continuationToken)}";
        }

        return new MessagesList(conversationList,nextUri);
    }
    
    public async Task DeleteMessage(string messageId, string conversationId)
    {
        try
        {
            await Container.DeleteItemAsync<MessageDto>(
                id: messageId,
                partitionKey: new PartitionKey(conversationId)
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
    
    private static MessageEntity ToEntity(string messageId, string conversationId, string senderUsername, string text, long unixTime)
    {
        return new MessageEntity(
            id: messageId, 
            ConversationId: conversationId, 
            UnixTime: unixTime, 
            SenderUsername: senderUsername,
            Text: text
        );
    }
    
    private static MessageDto ToMessage(MessageEntity messageEntity)
    {
        return new MessageDto(
            UnixTime: messageEntity.UnixTime, 
            SenderUsername: messageEntity.SenderUsername,
            Text: messageEntity.Text
        );
    }
}