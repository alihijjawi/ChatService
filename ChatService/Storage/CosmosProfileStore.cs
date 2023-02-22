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

    private Container Container => _cosmosClient.GetDatabase("profiles").GetContainer("sharedContainer");

    public async Task UpsertProfile(ProfileDto profile)
    {
        if (profile == null ||
            string.IsNullOrWhiteSpace(profile.UserName) ||
            string.IsNullOrWhiteSpace(profile.FirstName) ||
            string.IsNullOrWhiteSpace(profile.LastName)
           )
        {
            throw new ArgumentException($"Invalid profile {profile}", nameof(profile));
        }

        await Container.UpsertItemAsync(ToEntity(profile));
    }

    public async Task<ProfileDto?> GetProfile(string userName)
    {
        try
        {
            var entity = await Container.ReadItemAsync<ProfileEntity>(
                id: userName,
                partitionKey: new PartitionKey(userName),
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

    public async Task DeleteProfile(string userName)
    {
        try
        {
            await Container.DeleteItemAsync<ProfileDto>(
                id: userName,
                partitionKey: new PartitionKey(userName)
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
            profile.UserName,
            profile.FirstName,
            profile.LastName,
            profile.ProfilePictureId
        );
    }

    private static ProfileDto ToProfile(ProfileEntity entity)
    {
        return new ProfileDto(
            entity.UserName,
            entity.FirstName,
            entity.LastName,
            entity.ProfilePictureId
        );
    }
}