using System;
using System.Net.Http;
using ChatService.Client;

namespace ChatServiceFunctionalTests
{
    public static class TestUtils
    {
        public static ChatServiceClient CreateChatServiceClient()
        {
            var serviceUri = new Uri("https://localhost:7182");
            var httpClient = new HttpClient {BaseAddress = serviceUri};
            return new ChatServiceClient(httpClient);
        }
    }
}
