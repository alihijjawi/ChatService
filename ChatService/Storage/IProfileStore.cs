using ChatService.Dtos;

namespace ChatService.Storage;

public interface IProfileStore
{
    Task UpsertProfile(ProfileDto profile);
    Task<ProfileDto?> GetProfile(string username);
    Task DeleteProfile(string username);
}
