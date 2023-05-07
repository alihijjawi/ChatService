using ChatService.Dtos;
using Newtonsoft.Json;

namespace ChatService.Services.ServiceBus;

public class JsonProfileSerializer : IProfileSerializer
{
    public string SerializeProfile(ProfileDto profile)
    {
        return JsonConvert.SerializeObject(profile);
    }

    public ProfileDto DeserializeProfile(string serialized)
    {
        return JsonConvert.DeserializeObject<ProfileDto>(serialized);
    }
}