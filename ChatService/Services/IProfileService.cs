using ChatService.Dtos;

namespace ChatService.Services;

public interface IProfileService
{
    Task UpsertProfile(ProfileDto profile);
    Task<ProfileDto?> GetProfile(string username);
    Task DeleteProfile(string username);
}