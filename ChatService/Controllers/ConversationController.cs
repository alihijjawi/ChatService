using System.Text.Encodings.Web;
using System.Text.Json;
using System.Web;
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

        if (response == null) return NotFound();

        return CreatedAtAction(nameof(GetConversationList), null, response);
    }

    [HttpPost("{senderConversationId}/messages")]
    public async Task<ActionResult<SendMessageResponse>> SendMessage(string senderConversationId,
        SendMessageRequest messageRequest)
    {
        var response = await _chatManager.SendMessage(senderConversationId, messageRequest);

        if (response == null) return NotFound();

        return CreatedAtAction(nameof(GetMessageList), new { conversationId = senderConversationId }, response);
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