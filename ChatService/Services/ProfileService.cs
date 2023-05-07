using ChatService.Dtos;
using ChatService.Services.ServiceBus;
using ChatService.Storage;

namespace ChatService.Services;

public class ProfileService : IProfileService
{
    private readonly IProfileStore _profileStore;
    private readonly ICreateProfilePublisher _createProfilePublisher;

    public ProfileService(IProfileStore profileStore, ICreateProfilePublisher createProfilePublisher)
    {
        _profileStore = profileStore;
        _createProfilePublisher = createProfilePublisher;
    }
    
    public async Task EnqueueCreateProfile(ProfileDto profile)
    {
        await _createProfilePublisher.Send(profile);
    }

    public Task UpsertProfile(ProfileDto profile)
    {
        if (profile == null ||
            string.IsNullOrWhiteSpace(profile.UserName) ||
            string.IsNullOrWhiteSpace(profile.FirstName) ||
            string.IsNullOrWhiteSpace(profile.LastName)
           )
            throw new ArgumentException($"Invalid profile {profile}", nameof(profile));

        return _profileStore.UpsertProfile(profile);
    }

    public Task<ProfileDto?> GetProfile(string username)
    {
        return _profileStore.GetProfile(username);
    }

    public Task DeleteProfile(string username)
    {
        return _profileStore.DeleteProfile(username);
    }
}