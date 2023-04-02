using ChatService.Dtos;
using ChatService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Linq;

namespace ChatService.Controllers;

[ApiController]
[Route("[controller]")]
public class ConversationController : ControllerBase
{
    private readonly IChatManager _chatManager;

    public ConversationController(IChatManager chatManager)
    {
        _chatManager = chatManager;
    }

    [HttpPost]
    public async Task<ActionResult<StartConversationResponse>> StartConversation(
        StartConversationRequest conversationRequest)
    {
        var response = await _chatManager.StartConversation(conversationRequest);

        if (response.IsNull()) return NotFound();

        return CreatedAtAction(nameof(GetMessageList), new { conversationId = response.ConversationId }, response);
    }

    [HttpPost("{conversationId}/messages")]
    public async Task<ActionResult<SendMessageResponse>> SendMessage(string conversationId,
        SendMessageRequest messageRequest)
    {
        var response = await _chatManager.SendMessage(conversationId, messageRequest);

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
            await _chatManager.GetMessageList(conversationId, continuationToken, limit, lastSeenMessageTime);

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
            await _chatManager.GetConversationList(username, continuationToken, limit, lastSeenMessageTime);

        if (response.IsNull()) return NotFound();

        return Ok(response);
    }
}