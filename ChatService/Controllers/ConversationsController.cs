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

        if (response == null) return NotFound("The profile was not found.");

        return CreatedAtAction(nameof(GetConversationList), null, response);
        
        // todo returning CONFLICT
        // todo returning BADREQUEST
    }

    [HttpPost("{conversationId}/messages")]
    public async Task<ActionResult<SendMessageResponse>> SendMessage(string conversationId,
        SendMessageRequest messageRequest)
    {
        try
        {
            var response = await _chatManager.SendMessage(conversationId, messageRequest);
            return CreatedAtAction(nameof(GetMessageList), new { conversationId = conversationId }, response);
        }
        catch
        {
            return Conflict();
        }
        // todo returning NOTFOUND
        // todo returning BADREQUEST
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
        // todo returning NOTFOUND is not possible

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<ConversationsList>> GetConversationList([FromQuery] string username,
        [FromQuery] string? continuationToken,
        [FromQuery] string? limit,
        [FromQuery] string? lastSeenConversationTime)
    {
        var response =
            await _chatManager.GetConversationList(username, continuationToken, limit, lastSeenConversationTime);

        if (response == null) return NotFound();
        // todo returning NOTFOUND is not possible
        
        return Ok(response);
    }
}