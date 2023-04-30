using System.Data;
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
        try
        {
            var response = await _chatManager.StartConversation(conversationRequest);

            return CreatedAtAction(nameof(GetConversationList), null, response);
        } catch (DataException e) {
            Console.WriteLine(e);
            return NotFound($"Participants: '{conversationRequest.Participants}' not found");
        } catch (Exception e) {
            Console.WriteLine(e);
            return Conflict($"Conversation with participants: '{conversationRequest.Participants}' already exists");
        }
    }

    [HttpPost("{conversationId}/messages")]
    public async Task<ActionResult<SendMessageResponse>> SendMessage(string conversationId,
        SendMessageRequest messageRequest)
    {
        try
        {
            var response = await _chatManager.SendMessage(conversationId, messageRequest);
            return CreatedAtAction(nameof(GetMessageList), new { conversationId = conversationId }, response);
        } catch (DataException e) {
            Console.WriteLine(e);
            return NotFound($"Conversation with ConversationId: '{conversationId}' was not found");
        } catch (Exception e) {
            Console.WriteLine(e);
            return Conflict($"Message with Id: '{messageRequest.Id}' already exists");
        }
    }

    [HttpGet("{conversationId}/messages")]
    public async Task<ActionResult<MessagesList>> GetMessageList(string conversationId,
        [FromQuery] string? continuationToken,
        [FromQuery] string? limit,
        [FromQuery] string? lastSeenMessageTime)
    {
        try {
            var response =
                await _chatManager.GetMessageList(conversationId, continuationToken, limit, lastSeenMessageTime);
            
            return Ok(response);
        } catch (DataException e) {
            return NotFound($"Conversation with ConversationId: '{conversationId}' was not found");
        }
    }

    [HttpGet]
    public async Task<ActionResult<ConversationsList>> GetConversationList([FromQuery] string username,
        [FromQuery] string? continuationToken,
        [FromQuery] string? limit,
        [FromQuery] string? lastSeenConversationTime)
    {
        try {
            var response =
                await _chatManager.GetConversationList(username, continuationToken, limit, lastSeenConversationTime);
            return Ok(response);
        } catch (DataException e) {
            return NotFound($"A user with username {username} was not found");
        }
    }
}