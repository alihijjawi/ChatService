using ChatService.DataContracts;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Client
{

    public class ChatServiceClient : IChatServiceClient
    {
        private readonly HttpClient httpClient;

        public ChatServiceClient(Uri baseUri)
        {
            httpClient = new HttpClient()
            {
                BaseAddress = baseUri
            };
        }

        public ChatServiceClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task CreateProfile(CreateProfileRequest profileDto)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PostAsync("api/profile",
                    new StringContent(JsonConvert.SerializeObject(profileDto), Encoding.UTF8, "application/json"));
                await EnsureSuccessOrThrow(response);
            }
            catch (Exception e)
            {
                // make sure we don't catch our own exception we threw above
                if (e is ChatServiceException) throw;

                throw new ChatServiceException("Failed to reach chat service", e,
                    HttpStatusCode.InternalServerError);
            }
        }

        public async Task<UserProfileDto> GetProfile(string username)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync($"api/profile/{username}");
                await EnsureSuccessOrThrow(response);

                string content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserProfileDto>(content);
            }
            catch (JsonException e)
            {
                throw new ChatServiceException($"Failed to deserialize profile for user {username}", e, 
                    HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                // make sure we don't catch our own exception we threw above
                if (e is ChatServiceException) throw;

                throw new ChatServiceException("Failed to reach chat service", e, 
                    HttpStatusCode.InternalServerError);
            }
        }

        public async Task<AddConversationResponse> AddConversation(AddConversationRequest createConversationDto)
        {
            try
            {
                HttpResponseMessage response =
                    await httpClient.PostAsync("api/conversations",
                    new StringContent(JsonConvert.SerializeObject(createConversationDto), Encoding.UTF8, "application/json"));
                await EnsureSuccessOrThrow(response);

                string content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AddConversationResponse>(content);
            }
            catch (JsonException e)
            {
                throw new ChatServiceException("Failed to deserialize the response", e, 
                    HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                throw new ChatServiceException("Failed to reach chat service", e,
                    HttpStatusCode.InternalServerError);
            }
        }

        public Task<ListConversationsResponse> ListConversations(string username, int limit = 50, long lastSeenConversationTime= 0)
        {
            return ListConversationsByUri($"api/conversations/?username={username}&limit={limit}&lastSeenConversationTime={lastSeenConversationTime}");
        }

        public async Task<ListConversationsResponse> ListConversationsByUri(string uri)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(uri);
                await EnsureSuccessOrThrow(response);

                string content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ListConversationsResponse>(content);
            }
            catch (JsonException e)
            {
                throw new ChatServiceException("Failed to deserialize the response", e,
                    HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                throw new ChatServiceException("Failed to reach chat service", e,
                    HttpStatusCode.InternalServerError);
            }
        }

        public Task<ListMessagesResponse> ListMessages(string conversationId, int limit = 50, long lastSeenMessageTime = 0)
        {
            return ListMessagesByUri($"api/conversations/{conversationId}/messages?limit={limit}&lastSeenMessageTime={lastSeenMessageTime}");
        }

        public async Task<ListMessagesResponse> ListMessagesByUri(string uri)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(uri);
                await EnsureSuccessOrThrow(response);

                string content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ListMessagesResponse>(content);
            }
            catch (JsonException e)
            {
                throw new ChatServiceException("Failed to deserialize the response", e,
                    HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                throw new ChatServiceException("Failed to reach chat service", e,
                    HttpStatusCode.InternalServerError);
            }
        }

        public async Task SendMessage(string conversationId, SendMessageRequest messageDto)
        {
            HttpResponseMessage response =
                await httpClient.PostAsync($"api/conversations/{conversationId}/messages",
                new StringContent(JsonConvert.SerializeObject(messageDto), Encoding.UTF8, "application/json"));

            await EnsureSuccessOrThrow(response);
        }

        public async Task<UploadImageResponse> UploadImage(Stream stream)
        {
            HttpContent fileStreamContent = new StreamContent(stream);
            fileStreamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "file",
                FileName = "NotNeeded"
            };
            fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(fileStreamContent);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"api/images")
                {
                    Content = formData
                };

                try 
                { 
                    HttpResponseMessage response = await httpClient.SendAsync(request);
                    await EnsureSuccessOrThrow(response);

                    string content = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        throw new ChatServiceException($"Unexpected response body", HttpStatusCode.InternalServerError);
                    }
                    try
                    {
                        return JsonConvert.DeserializeObject<UploadImageResponse>(content);
                    }
                    catch (JsonException e)
                    {
                        throw new ChatServiceException($"Failed to deserialize the response content: {content}", e,
                            HttpStatusCode.InternalServerError);
                    }
                }
                catch (Exception e)
                {
                    throw new ChatServiceException("Failed to reach chat service", e,
                        HttpStatusCode.InternalServerError);
                }
            }
        }

        public async Task<DownloadImageResponse> DownloadImage(string imageId)
        {
            using (HttpResponseMessage response = await httpClient.GetAsync($"api/images/{imageId}"))
            {
                var bytes = await response.Content.ReadAsByteArrayAsync();
                return new DownloadImageResponse(bytes);
            }
        }

        public Task DeleteImage(string imageId)
        {
            return httpClient.DeleteAsync($"api/image/{imageId}");
        }

        private async Task EnsureSuccessOrThrow(HttpResponseMessage responseMessage)
        {
            if (!responseMessage.IsSuccessStatusCode)
            {
                string message = $"{responseMessage.ReasonPhrase}, {await responseMessage.Content.ReadAsStringAsync()}";
                throw new ChatServiceException(message, responseMessage.StatusCode);
            }
        }
    }
}
