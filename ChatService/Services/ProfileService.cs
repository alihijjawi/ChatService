using ChatService.Dtos;
using ChatService.Storage;

namespace ChatService.Services;

public class ProfileService : IProfileService
{
    private readonly IProfileStore _profileStore;

    public ProfileService(IProfileStore profileStore)
    {
        _profileStore = profileStore;
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