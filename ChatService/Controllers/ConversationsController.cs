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

    public ConversationController(IConversationsService conversationsService)
    {
        _conversationsService = conversationsService;
    }

    [HttpPost]
    public async Task<ActionResult<StartConversationResponse>> StartConversation()
    {
        var response = await _conversationsService.StartConversation();
        
        if (response.IsNull())
        {
            return NotFound();
        }
        
        return CreatedAtAction(nameof(GetConversation), new {conversationId = response.ConversationId}, response);
    }

    [HttpPost("{conversationId}/messages")]
    public async Task<ActionResult<SendMessageResponse>> SendMessage(string conversationId)
    {
        var response = await _conversationsService.SendMessage();
        
        if (response.IsNull())
        {
            return NotFound();
        }
        
        return CreatedAtAction(nameof(GetConversation), new {conversationId}, response);
    }

    [HttpGet("{conversationId}/messages")]
    public async Task<ActionResult<MessagesList>> GetConversation(string conversationId, 
        [FromQuery] string continuationToken,
        [FromQuery] string limit, 
        [FromQuery] string lastSeenMessageTime)
    {
        var response = await _conversationsService.GetConversation();

        if (response.IsNull())
        {
            return NotFound();
        }
        
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<ConversationsList>> GetConversationList([FromQuery] string username,
        [FromQuery] string continuationToken,
        [FromQuery] string limit,
        [FromQuery] string lastSeenMessageTime)
    {
        var response = await _conversationsService.GetConversationList();
        
        if (response.IsNull())
        {
            return NotFound();
        }
        
        return Ok(response);
    }
}