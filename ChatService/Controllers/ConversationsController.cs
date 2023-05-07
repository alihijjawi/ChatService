using System.Data;
using System.Diagnostics;
using ChatService.Dtos;
using ChatService.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConversationsController : ControllerBase
{
    private readonly IChatManager _chatManager;
    private readonly ILogger<ProfileController> _logger;
    private readonly TelemetryClient _telemetry;

    public ConversationsController(IChatManager chatManager, ILogger<ProfileController> logger, TelemetryClient telemetry)
    {
        _chatManager = chatManager;
        _logger = logger;
        _telemetry = telemetry;
    }

    [HttpPost]
    public async Task<ActionResult<StartConversationResponse>> StartConversation(
        StartConversationRequest conversationRequest)
    {
        using (_logger.BeginScope("{Request}", conversationRequest))
        {
            _logger.LogInformation("Starting conversation for users {user1} and {user2}", conversationRequest.Participants[0], conversationRequest.Participants[1]);

            try
            {
                var timer = new Stopwatch();
                timer.Start();
                // await _conversationService.EnqueueCreateProfile(conversationRequest);
                // for some reason it was not finding the service bus
                var response = await _chatManager.StartConversation(conversationRequest);
                timer.Stop();
                
                _telemetry.TrackEvent("Creating a Conversation");
                _telemetry.TrackMetric("Creating a Conversation time", timer.ElapsedMilliseconds);
            
                _logger.LogInformation("Conversation created: {ConversationId}", response.Id);
                return CreatedAtAction(nameof(GetConversationList), null, response);
            }
            catch (DataException e)
            {
                _logger.LogInformation("Failed to create a conversation with username(s) {user1} or/and {user2} is/are missing", conversationRequest.Participants[0], conversationRequest.Participants[1]);
                return NotFound($"One or Both Participants: '{conversationRequest.Participants}' not found");
            }
            catch (Exception e)
            {
                _logger.LogInformation("Failed to create a duplicate conversation for usernames {user1} and {user2}", conversationRequest.Participants[0], conversationRequest.Participants[1]);
                return Conflict($"Conversation with participants: '{conversationRequest.Participants}' already exists");
            }
        }
    }

    [HttpPost("{conversationId}/messages")]
    public async Task<ActionResult<SendMessageResponse>> SendMessage(string conversationId,
        SendMessageRequest messageRequest)
    {
        using (_logger.BeginScope("{ConversationId}", conversationId))
        {
            _logger.LogInformation("Sending message {MessageId} to conversation {ConversationId} from user {sender}", messageRequest.Id, conversationId, messageRequest.SenderUsername);

            try
            {
                var timer = new Stopwatch();
                timer.Start();
                var response = await _chatManager.SendMessage(conversationId, messageRequest);
                timer.Stop();

                _telemetry.TrackEvent("Sending a Message");
                _telemetry.TrackMetric("Sending a Message time", timer.ElapsedMilliseconds);
            
                _logger.LogInformation("Message {MessageId} = {Text} sent at {Time}", messageRequest.Id, messageRequest.Text, response.CreatedUnixTime);
                return CreatedAtAction(nameof(GetMessageList), new { conversationId }, response);
            }
            catch (DataException e)
            {
                _logger.LogInformation("Failed to send message to a non-existent conversation {ConversationId}", conversationId);
                return NotFound($"Conversation with ConversationId: '{conversationId}' was not found");
            }
            catch (Exception e)
            {
                _logger.LogInformation("Failed to send a duplicate message {MessageId}", messageRequest.Id);
                return Conflict($"Message with Id: '{messageRequest.Id}' already exists");
            }
        }
    }

    [HttpGet("{conversationId}/messages")]
    public async Task<ActionResult<MessagesList>> GetMessageList(string conversationId,
        [FromQuery] string? continuationToken,
        [FromQuery] string? limit,
        [FromQuery] string? lastSeenMessageTime)
    {
        using (_logger.BeginScope("{ConversationId}", conversationId))
        {
            _logger.LogInformation("Fetching messages of conversation {ConversationId}", conversationId);

            try {
                var timer = new Stopwatch();
                timer.Start();
                var response =
                    await _chatManager.GetMessageList(conversationId, continuationToken, limit, lastSeenMessageTime);
                timer.Stop();

                _telemetry.TrackEvent("Fetching Messages");
                _telemetry.TrackMetric("Fetching Messages time", timer.ElapsedMilliseconds);
            
                _logger.LogInformation("Fetched Messages of conversation {ConversationId} ", conversationId);
                return Ok(response);
            } 
            catch (DataException e)
            {
                _logger.LogInformation("Failed to fetch a non-existent {ConversationId} conversation's messages", conversationId);
                return NotFound($"Conversation with ConversationId: '{conversationId}' was not found");
            }
        }
    }

    [HttpGet]
    public async Task<ActionResult<ConversationsList>> GetConversationList([FromQuery] string username,
        [FromQuery] string? continuationToken,
        [FromQuery] string? limit,
        [FromQuery] string? lastSeenConversationTime)
    {
        using (_logger.BeginScope("{Username}", username))
        {
            _logger.LogInformation("Fetching conversation of user {Username}", username);

            try
            {
                var timer = new Stopwatch();
                timer.Start();
                var response =
                    await _chatManager.GetConversationList(username, continuationToken, limit,
                        lastSeenConversationTime);
                timer.Stop();

                _telemetry.TrackEvent("Fetching Conversations");
                _telemetry.TrackMetric("Fetching Conversations time", timer.ElapsedMilliseconds);

                _logger.LogInformation("Fetched Conversations of user {Username} ", username);
                return Ok(response);
            }
            catch (DataException e)
            {
                _logger.LogInformation("Failed to fetch a non-existent {Username} Profile's conversations", username);
                return NotFound($"A user with username {username} was not found");
            }
        }
    }
}