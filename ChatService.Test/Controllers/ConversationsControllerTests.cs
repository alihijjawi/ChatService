using System.Data;
using System.Net;
using System.Text;
using System.Web;
using ChatService.Dtos;
using ChatService.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;

namespace ChatService.Test.Controllers;

public class ConversationsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly Mock<IChatManager> _chatManagerMock = new();
    private readonly HttpClient _httpClient;

    public ConversationsControllerTests(WebApplicationFactory<Program> factory)
    {
        _httpClient = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services => { services.AddSingleton(_chatManagerMock.Object); });
        }).CreateClient();
    }

    [Fact]
    public async Task StartConversation_Success()
    {
        string[] participants = new[] { "foo", "bar" };
        var sendMessageRequest = new SendMessageRequest(Guid.NewGuid().ToString(), "foo", "test");
        
        var startConversationRequest = new StartConversationRequest(participants, sendMessageRequest);
        
        _chatManagerMock.Setup(m => m.StartConversation(It.IsAny<StartConversationRequest>()))
            .ReturnsAsync(new StartConversationResponse(Guid.NewGuid().ToString(), long.MinValue));
        
        var response = await _httpClient.PostAsync("api/Conversations",
            new StringContent(JsonConvert.SerializeObject(startConversationRequest), Encoding.Default, "application/json"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("http://localhost/api/Conversations", response.Headers.GetValues("Location").First());

        _chatManagerMock.Verify(mock => mock.StartConversation(It.IsAny<StartConversationRequest>()), Times.Once);
    }
    
    [Fact]
    public async Task StartConversation_ProfileNotFound()
    {
        string[] participants = new[] { "foo", "bar" };
        var sendMessageRequest = new SendMessageRequest(Guid.NewGuid().ToString(), "foo", "test");
        var startConversationRequest = new StartConversationRequest(participants, sendMessageRequest);

        _chatManagerMock.Setup(m => m.StartConversation(It.IsAny<StartConversationRequest>()))!
            .ThrowsAsync(new DataException());

        var response = await _httpClient.PostAsync("api/Conversations",
            new StringContent(JsonConvert.SerializeObject(startConversationRequest), Encoding.Default, "application/json"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        _chatManagerMock.Verify(mock => mock.StartConversation(startConversationRequest), Times.Never);
    }
    
    [Fact]
    public async Task StartConversation_Conflict()
    {
        string[] participants = new[] { "foo", "bar" };
        var sendMessageRequest = new SendMessageRequest(Guid.NewGuid().ToString(), "foo", "test");
        var startConversationRequest = new StartConversationRequest(participants, sendMessageRequest);

        _chatManagerMock.Setup(m => m.StartConversation(It.IsAny<StartConversationRequest>()))!
            .ThrowsAsync(new Exception());

        var response = await _httpClient.PostAsync("api/Conversations",
            new StringContent(JsonConvert.SerializeObject(startConversationRequest), Encoding.Default, "application/json"));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        _chatManagerMock.Verify(mock => mock.StartConversation(startConversationRequest), Times.Never);
    }
    
    [Fact]
    public async Task SendMessage_Sucess()
    {
        var conversationId = Guid.NewGuid().ToString();
        var sendMessageRequest = new SendMessageRequest(Guid.NewGuid().ToString(), "foo", "test");

        var response = await _httpClient.PostAsync($"api/Conversations/{conversationId}/messages",
            new StringContent(JsonConvert.SerializeObject(sendMessageRequest), Encoding.Default, "application/json"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal($"http://localhost/api/Conversations/{conversationId}/messages", response.Headers.GetValues("Location").First());

        _chatManagerMock.Verify(mock => mock.SendMessage(conversationId, sendMessageRequest), Times.Once);
    }
    
    [Fact]
    public async Task SendMessage_Conflict()
    {
        var conversationId = Guid.NewGuid().ToString();
        var sendMessageRequest = new SendMessageRequest(Guid.NewGuid().ToString(), "foo", "test");

        _chatManagerMock.Setup(m => m.SendMessage(conversationId, sendMessageRequest))
            .ThrowsAsync(new Exception());
        
        var response = await _httpClient.PostAsync($"api/Conversations/{conversationId}/messages",
            new StringContent(JsonConvert.SerializeObject(sendMessageRequest), Encoding.Default, "application/json"));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        _chatManagerMock.Verify(mock => mock.SendMessage(conversationId, sendMessageRequest), Times.Once);
    }
    
    [Fact]
    public async Task SendMessage_NotFound()
    {
        var conversationId = Guid.NewGuid().ToString();
        var sendMessageRequest = new SendMessageRequest(Guid.NewGuid().ToString(), "foo", "test");

        _chatManagerMock.Setup(m => m.SendMessage(conversationId, sendMessageRequest))
            .ThrowsAsync(new DataException());
        
        var response = await _httpClient.PostAsync($"api/Conversations/{conversationId}/messages",
            new StringContent(JsonConvert.SerializeObject(sendMessageRequest), Encoding.Default, "application/json"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        _chatManagerMock.Verify(mock => mock.SendMessage(conversationId, sendMessageRequest), Times.Once);
    }
    
    [Fact]
    public async Task GetMessageList_Success()
    {
        var conversationId = Guid.NewGuid().ToString();
        var messagesList = new MessagesList(Array.Empty<MessageDto>(), conversationId);
        _chatManagerMock.Setup(m => m.GetMessageList(conversationId, null, null, null))
            .ReturnsAsync(messagesList);

        var response = await _httpClient.GetAsync($"api/Conversations/{conversationId}/messages");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        Assert.Equal(messagesList, JsonConvert.DeserializeObject<MessagesList>(json));
    }
    
    [Fact]
    public async Task GetMessageList_NotFound()
    {
        var conversationId = Guid.NewGuid().ToString();
        _chatManagerMock.Setup(m => m.GetMessageList(conversationId, null, null, null))!
            .ThrowsAsync(new DataException());

        var response = await _httpClient.GetAsync($"api/Conversations/{conversationId}/messages");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task GetConversationList_Success()
    {
        var username = Guid.NewGuid().ToString();
        var conversationsList = new ConversationsList(Array.Empty<ConversationDto>(), "test");
        _chatManagerMock.Setup(m => m.GetConversationList(username, null, null, null))
            .ReturnsAsync(conversationsList);
        
        var uriBuilder = new UriBuilder("https://localhost/api/Conversations");
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["username"] = username;
        uriBuilder.Query = query.ToString();
        var response = await _httpClient.GetAsync(uriBuilder.Uri);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        Assert.Equal(conversationsList, JsonConvert.DeserializeObject<ConversationsList>(json));
    }
    
    [Fact]
    public async Task GetConversationList_NotFound()
    {
        var username = Guid.NewGuid().ToString();
        _chatManagerMock.Setup(m => m.GetConversationList(username, null, null, null))!
            .ThrowsAsync(new DataException());

        var uriBuilder = new UriBuilder("https://localhost/api/Conversations");
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["username"] = username;
        uriBuilder.Query = query.ToString();
        var response = await _httpClient.GetAsync(uriBuilder.Uri);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}