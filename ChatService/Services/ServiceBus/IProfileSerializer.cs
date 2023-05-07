using ChatService.Dtos;

namespace ChatService.Services.ServiceBus;

public interface IProfileSerializer
{
    string SerializeProfile(ProfileDto profile);
    ProfileDto DeserializeProfile(string serialized);
}