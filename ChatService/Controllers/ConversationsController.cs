using System.Text.Encodings.Web;
using System.Text.Json;
using System.Web;
using ChatService.Dtos;
using ChatService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Linq;

namespace ChatService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConversationsController : ControllerBase
{
    private readonly IChatManager _chatManager;

    public ConversationsController(IChatManager chatManager)
    {
        _chatManager = chatManager;
    }

    [HttpPost]
    public async Task<ActionResult<StartConversationResponse>> StartConversation(
        StartConversationRequest conversationRequest)
    {
        var response = await _chatManager.StartConversation(conversationRequest);

        if (response == null) return NotFound("abcdefg");

        return CreatedAtAction(nameof(GetConversationList), null, response);
    }

    [HttpPost("{conversationId}/messages")]
    public async Task<ActionResult<SendMessageResponse>> SendMessage(string conversationId,
        SendMessageRequest messageRequest)
    {
        var response = await _chatManager.SendMessage(conversationId, messageRequest);

        if (response == null) return NotFound();

        return CreatedAtAction(nameof(GetMessageList), new { conversationId = conversationId }, response);
    }

    [HttpGet("{conversationId}/messages")]
    public async Task<ActionResult<MessagesList>> GetMessageList(string conversationId,
        [FromQuery] string? continuationToken,
        [FromQuery] string? limit,
        [FromQuery] string? lastSeenMessageTime)
    {
        var response =
            await _chatManager.GetMessageList(conversationId, continuationToken, limit, lastSeenMessageTime);

        if (response == null) return NotFound();

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<ConversationsList>> GetConversationList([FromQuery] string username,
        [FromQuery] string? continuationToken,
        [FromQuery] string? limit,
        [FromQuery] string? lastSeenMessageTime)
    {
        var response =
            await _chatManager.GetConversationList(username, continuationToken, limit, lastSeenMessageTime);

        if (response == null) return NotFound();
        
        return Ok(response);
    }
}