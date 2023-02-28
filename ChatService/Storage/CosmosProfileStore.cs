using System.Net;
using ChatService.Dtos;
using ChatService.Storage.Entities;
using Microsoft.Azure.Cosmos;

namespace ChatService.Storage;

public class CosmosProfileStore : IProfileStore
{
    private readonly CosmosClient _cosmosClient;

    public CosmosProfileStore(CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
    }

    // DRY
    private Container Container => _cosmosClient.GetDatabase("profilesDb").GetContainer("profiles");

    public async Task UpsertProfile(ProfileDto profile)
    {
        await Container.UpsertItemAsync(ToEntity(profile));
    }

    public async Task<ProfileDto?> GetProfile(string username)
    {
        try
        {
            var entity = await Container.ReadItemAsync<ProfileEntity>(
                id: username,
                partitionKey: new PartitionKey(username),
                new ItemRequestOptions
                {
                    ConsistencyLevel = ConsistencyLevel.Session
                }
            );
            return ToProfile(entity);
        }
        catch (CosmosException e)
        {
            if (e.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            throw;
        }
    }

    public async Task DeleteProfile(string username)
    {
        try
        {
            await Container.DeleteItemAsync<ProfileDto>(
                id: username,
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

    private static ProfileEntity ToEntity(ProfileDto profile)
    {
        return new ProfileEntity(
            PartitionKey: profile.UserName,
            id: profile.UserName,
            profile.FirstName,
            profile.LastName,
            profile.ProfilePictureId
        );
    }

    private static ProfileDto ToProfile(ProfileEntity entity)
    {
        return new ProfileDto(
            UserName: entity.id,
            entity.FirstName,
            entity.LastName,
            entity.ProfilePictureId
        );
    }
}