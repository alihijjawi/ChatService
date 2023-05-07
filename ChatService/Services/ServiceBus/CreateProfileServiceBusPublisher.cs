using Azure.Messaging.ServiceBus;
using ChatService.Dtos;
using ChatService.Settings;
using Microsoft.Extensions.Options;

namespace ChatService.Services.ServiceBus;

public class CreateProfileServiceBusPublisher : ICreateProfilePublisher
{
    private readonly IProfileSerializer _profileSerializer;
    private readonly ServiceBusSender _sender;

    public CreateProfileServiceBusPublisher(
        ServiceBusClient serviceBusClient,
        IProfileSerializer profileSerializer,
        IOptions<ServiceBusSettings> options)
    {
        _profileSerializer = profileSerializer;
        _sender = serviceBusClient.CreateSender(options.Value.CreateProfileQueueName);
    }

    public Task Send(ProfileDto profile)
    {
        var serialized = _profileSerializer.SerializeProfile(profile);
        return _sender.SendMessageAsync(new ServiceBusMessage(serialized));
    }
}