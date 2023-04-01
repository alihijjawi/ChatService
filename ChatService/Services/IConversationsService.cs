using ChatService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Services;

public interface IConversationsService
{
    Task<StartConversationResponse> StartConversation();

    Task<SendMessageResponse> SendMessage();

    Task<ConversationDto> GetConversation();

    Task<ConversationsList> GetConversationList();
}