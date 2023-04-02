using ChatService.Dtos;
using ChatService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Linq;

namespace ChatService.Controllers;

[ApiController]
[Route("[controller]")]
public class ConversationController : ControllerBase
{
    private readonly IConversationsService _conversationsService;

    private readonly IMessagesService _messagesService;

    public ConversationController(IConversationsService conversationsService, IMessagesService messagesService)
    {
        _conversationsService = conversationsService;
        _messagesService = messagesService;
    }

    [HttpPost]
    public async Task<ActionResult<StartConversationResponse>> StartConversation(
        StartConversationRequest conversationRequest)
    {
        var response = await _conversationsService.StartConversation(conversationRequest);

        if (response.IsNull()) return NotFound();

        return CreatedAtAction(nameof(GetMessageList), new { conversationId = response.ConversationId }, response);
    }

    [HttpPost("{conversationId}/messages")]
    public async Task<ActionResult<SendMessageResponse>> SendMessage(string conversationId,
        SendMessageRequest messageRequest)
    {
        var response = await _messagesService.SendMessage(conversationId, messageRequest);

        if (response.IsNull()) return NotFound();

        return CreatedAtAction(nameof(GetMessageList), new { conversationId }, response);
    }

    [HttpGet("{conversationId}/messages")]
    public async Task<ActionResult<MessagesList>> GetMessageList(string conversationId,
        [FromQuery] string continuationToken,
        [FromQuery] string limit,
        [FromQuery] string lastSeenMessageTime)
    {
        var response =
            await _messagesService.GetMessageList(conversationId, continuationToken, limit, lastSeenMessageTime);

        if (response.IsNull()) return NotFound();

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<ConversationsList>> GetConversationList([FromQuery] string username,
        [FromQuery] string continuationToken,
        [FromQuery] string limit,
        [FromQuery] string lastSeenMessageTime)
    {
        var response =
            await _conversationsService.GetConversationList(username, continuationToken, limit, lastSeenMessageTime);

        if (response.IsNull()) return NotFound();

        return Ok(response);
    }
}