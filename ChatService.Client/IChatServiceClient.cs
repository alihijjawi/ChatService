using System.IO;
using System.Threading.Tasks;
using ChatService.DataContracts;

namespace ChatService.Client
{
    public interface IChatServiceClient
    {
        // profile
        Task CreateProfile(CreateProfileRequest profileDto);
        Task<UserProfileDto> GetProfile(string username);

        // images
        Task DeleteImage(string imageId);
        Task<UploadImageResponse> UploadImage(Stream stream);
        Task<DownloadImageResponse> DownloadImage(string imageId);

        // conversations
        Task<AddConversationResponse> AddConversation(AddConversationRequest createConversationDto);
        Task<ListConversationsResponse> ListConversations(string username, int limit = 50, long lastSeenConversationTime = 0);
        Task<ListConversationsResponse> ListConversationsByUri(string uri);

        // messages
        Task SendMessage(string conversationId, SendMessageRequest messageDto);
        Task<ListMessagesResponse> ListMessages(string conversationId, int limit = 50, long lastSeenMessageTime = 0);
        Task<ListMessagesResponse> ListMessagesByUri(string uri);
    }
}