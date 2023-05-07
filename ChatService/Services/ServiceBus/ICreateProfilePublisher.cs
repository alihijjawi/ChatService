using ChatService.Dtos;

namespace ChatService.Services.ServiceBus;

public interface ICreateProfilePublisher
{
    Task Send(ProfileDto profile);
}
