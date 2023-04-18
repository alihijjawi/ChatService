using System.Net;
using ChatService.Client;
using ChatService.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChatServiceFunctionalTests
{
    [TestClass]
    [TestCategory("Integration")]
    [TestCategory("Functional")]
    public class ConversationsControllerIntegrationTests
    {
        private ChatServiceClient _chatServiceClient;

        [TestInitialize]
        public void TestInitialize()
        {
            _chatServiceClient = TestUtils.CreateChatServiceClient();
        }

        private AddConversationRequest NewConversationDto(string sender, string firstMessage, params string[] participants)
        {
            return new AddConversationRequest(new SendMessageRequest(Guid.NewGuid().ToString(), firstMessage, sender),
                participants);
        }

        [TestMethod]
        public async Task CreateListConversations()
        {
            string participant1 = RandomString();
            string participant2 = RandomString();
            string participant3 = RandomString();
            string participant4 = RandomString();

            await Task.WhenAll(
                _chatServiceClient.CreateProfile(new CreateProfileRequest { Username = participant1, FirstName = "Participant", LastName = "1"}),
                _chatServiceClient.CreateProfile(new CreateProfileRequest { Username = participant2, FirstName = "Participant", LastName = "2" }),
                _chatServiceClient.CreateProfile(new CreateProfileRequest { Username = participant3, FirstName = "Participant", LastName = "3" }),
                _chatServiceClient.CreateProfile(new CreateProfileRequest { Username = participant4, FirstName = "Participant", LastName = "4" })
            );

            await _chatServiceClient.AddConversation(NewConversationDto(participant1, "Hello", participant1, participant2));
            await _chatServiceClient.AddConversation(NewConversationDto(participant1, "Hello", participant1, participant3));
            await _chatServiceClient.AddConversation(NewConversationDto(participant1, "Hello", participant1, participant4));

            ListConversationsResponse participant1ConversationsDto = await _chatServiceClient.ListConversations(participant1, limit:2);
            Assert.AreEqual(2, participant1ConversationsDto.Conversations.Count);
            Assert.AreEqual(participant4, participant1ConversationsDto.Conversations[0].Recipient.Username);
            Assert.AreEqual(participant3, participant1ConversationsDto.Conversations[1].Recipient.Username);

            ListConversationsResponse participant2ConversationsDto = await _chatServiceClient.ListConversations(participant2);
            Assert.AreEqual(1, participant2ConversationsDto.Conversations.Count);
            Assert.AreEqual(participant1, participant2ConversationsDto.Conversations[0].Recipient.Username);

            // fetch next page
            participant1ConversationsDto = await _chatServiceClient.ListConversationsByUri(participant1ConversationsDto.NextUri);
            Assert.AreEqual(1, participant1ConversationsDto.Conversations.Count);
            Assert.AreEqual(participant2, participant1ConversationsDto.Conversations[0].Recipient.Username);
            Assert.IsTrue(string.IsNullOrWhiteSpace(participant1ConversationsDto.NextUri));
        }

        [TestMethod]
        public async Task CreateConversationAddsAMessage()
        {
            string participant1 = RandomString();
            string participant2 = RandomString();

            await Task.WhenAll(
                _chatServiceClient.CreateProfile(new CreateProfileRequest { Username = participant1, FirstName = "Participant", LastName = "1" }),
                _chatServiceClient.CreateProfile(new CreateProfileRequest { Username = participant2, FirstName = "Participant", LastName = "2" })
            );

            var createConversationDto = NewConversationDto(participant1, "Hello", participant1, participant2);
            var conversationDto = await _chatServiceClient.AddConversation(createConversationDto);
            ListMessagesResponse listMessagesDto = await _chatServiceClient.ListMessages(conversationDto.Id);
            Assert.AreEqual(1, listMessagesDto.Messages.Count);
            Assert.AreEqual(createConversationDto.FirstMessage.Text, listMessagesDto.Messages[0].Text);
        }

        [TestMethod]
        public async Task ListConversationsWithMinDateTime()
        {
            string participant1 = RandomString();
            string participant2 = RandomString();
            string participant3 = RandomString();
            string participant4 = RandomString();

            await Task.WhenAll(
                _chatServiceClient.CreateProfile(new CreateProfileRequest { Username = participant1, FirstName = "Participant", LastName = "1"}),
                _chatServiceClient.CreateProfile(new CreateProfileRequest { Username = participant2, FirstName = "Participant", LastName = "2" }),
                _chatServiceClient.CreateProfile(new CreateProfileRequest { Username = participant3, FirstName = "Participant", LastName = "3" }),
                _chatServiceClient.CreateProfile(new CreateProfileRequest { Username = participant4, FirstName = "Participant", LastName = "4" })
            );

            await _chatServiceClient.AddConversation(NewConversationDto(participant1, "Hello", participant1, participant2));
            await _chatServiceClient.AddConversation(NewConversationDto(participant1, "Hello", participant1, participant3));

            ListConversationsResponse conversationsDto = await _chatServiceClient.ListConversations(participant1, limit:2);
            Assert.AreEqual(2, conversationsDto.Conversations.Count);
            Assert.AreEqual(participant3, conversationsDto.Conversations[0].Recipient.Username);
            
            await _chatServiceClient.AddConversation(NewConversationDto(participant1, "Hello", participant1, participant4));
            conversationsDto = await _chatServiceClient.ListConversations(participant1, limit:10, conversationsDto.Conversations[0].LastModifiedUnixTime);
            
            Assert.AreEqual(1, conversationsDto.Conversations.Count);
            Assert.AreEqual(participant4, conversationsDto.Conversations[0].Recipient.Username);
        }

        [TestMethod]
        public async Task AddListMessages()
        {
            string participant1 = RandomString();
            string participant2 = RandomString();

            await Task.WhenAll(
                _chatServiceClient.CreateProfile(new CreateProfileRequest { Username = participant1, FirstName = "Participant", LastName = "1" }),
                _chatServiceClient.CreateProfile(new CreateProfileRequest { Username = participant2, FirstName = "Participant", LastName = "2" })
                );

            var createConversationDto = NewConversationDto(participant1, "Hello", participant1, participant2);
            var conversationDto = await _chatServiceClient.AddConversation(createConversationDto);

            var message1 = createConversationDto.FirstMessage;
            var message2 = new SendMessageRequest(RandomString(), "What's up?", participant1);
            var message3 = new SendMessageRequest(RandomString(), "Not much!", participant2);
            await _chatServiceClient.SendMessage(conversationDto.Id, message2);
            await _chatServiceClient.SendMessage(conversationDto.Id, message3);

            // list two top messages
            ListMessagesResponse listMessagesDto = await _chatServiceClient.ListMessages(conversationDto.Id, 2);
            Assert.AreEqual(2, listMessagesDto.Messages.Count);
            Assert.AreEqual(message3.Text, listMessagesDto.Messages[0].Text);
            Assert.AreEqual(message2.Text, listMessagesDto.Messages[1].Text);

            // list older message
            listMessagesDto = await _chatServiceClient.ListMessagesByUri(listMessagesDto.NextUri);
            Assert.AreEqual(1, listMessagesDto.Messages.Count);
            Assert.AreEqual(message1.Text, listMessagesDto.Messages[0].Text);
            Assert.IsTrue(string.IsNullOrWhiteSpace(listMessagesDto.NextUri));
        }

        [TestMethod]
        public async Task AddMessageUpdatesConversationOrder()
        {
            string participant1 = RandomString();
            string participant2 = RandomString();
            string participant3 = RandomString();

            await Task.WhenAll(
                _chatServiceClient.CreateProfile(new CreateProfileRequest { Username = participant1, FirstName = "Participant", LastName = "1" }),
                _chatServiceClient.CreateProfile(new CreateProfileRequest { Username = participant2, FirstName = "Participant", LastName = "2" }),
                _chatServiceClient.CreateProfile(new CreateProfileRequest { Username = participant3, FirstName = "Participant", LastName = "3" })
            );

            await _chatServiceClient.AddConversation(NewConversationDto(participant1, "Hello", participant1, participant2));
            await _chatServiceClient.AddConversation(NewConversationDto(participant1, "Hello", participant1, participant3));

            ListConversationsResponse conversationsDto = await _chatServiceClient.ListConversations(participant1, limit: 2);
            Assert.AreEqual(2, conversationsDto.Conversations.Count);
            Assert.AreEqual(participant3, conversationsDto.Conversations[0].Recipient.Username);
            Assert.AreEqual(participant2, conversationsDto.Conversations[1].Recipient.Username);

            // sending a message to participant2 should put it's conversation on top
            var message = new SendMessageRequest(RandomString(), "Hello", participant1);
            await _chatServiceClient.SendMessage(conversationsDto.Conversations[1].Id, message);

            conversationsDto = await _chatServiceClient.ListConversations(participant1, limit: 1);
            Assert.AreEqual(1, conversationsDto.Conversations.Count);
            Assert.AreEqual(participant2, conversationsDto.Conversations[0].Recipient.Username);
        }

        [TestMethod]
        public async Task ListMessagesWithMinDateTime()
        {
            //We require a viable profile object in order to start a conversation so this test was modified slightly
            string participant1 = RandomString();
            string participant2 = RandomString();
            
            await Task.WhenAll(
                _chatServiceClient.CreateProfile(new CreateProfileRequest { Username = participant1, FirstName = "Participant", LastName = "1"}),
                _chatServiceClient.CreateProfile(new CreateProfileRequest { Username = participant2, FirstName = "Participant", LastName = "2" })
            );
            
            var createConversationDto = NewConversationDto(participant1, "Hello", participant1, participant2);

            var conversationDto = await _chatServiceClient.AddConversation(createConversationDto);
            var message1 = createConversationDto.FirstMessage;
            var message2 = new SendMessageRequest(RandomString(), "What's up?", participant1);
            var message3 = new SendMessageRequest(RandomString(), "Not much!", participant2);
            await _chatServiceClient.SendMessage(conversationDto.Id, message2);
            await _chatServiceClient.SendMessage(conversationDto.Id, message3);

            ListMessagesResponse listMessagesDto = await _chatServiceClient.ListMessages(conversationDto.Id, 10);
            Assert.AreEqual(3, listMessagesDto.Messages.Count);

            var message4 = new SendMessageRequest(RandomString(), "OK...", participant1);
            await _chatServiceClient.SendMessage(conversationDto.Id, message4);
            
            // list all messages received after the datetime of message3
            listMessagesDto = await _chatServiceClient.ListMessages(conversationDto.Id, 10, lastSeenMessageTime:listMessagesDto.Messages[0].UnixTime);
            Assert.AreEqual(1, listMessagesDto.Messages.Count);
            Assert.AreEqual(message4.Text, listMessagesDto.Messages[0].Text);
            Assert.IsTrue(string.IsNullOrWhiteSpace(listMessagesDto.NextUri));
        }

        [TestMethod]
        public async Task AddingDuplicateMessageReturnsConflict()
        {
            //We require a viable profile object in order to start a conversation so this test was modified slightly
            string participant1 = RandomString();
            string participant2 = RandomString();
            
            await Task.WhenAll(
                _chatServiceClient.CreateProfile(new CreateProfileRequest { Username = participant1, FirstName = "Participant", LastName = "1"}),
                _chatServiceClient.CreateProfile(new CreateProfileRequest { Username = participant2, FirstName = "Participant", LastName = "2" })
            );
            
            var createConversationDto = NewConversationDto(participant1, "Hello", participant1, participant2);


            var conversationDto = await _chatServiceClient.AddConversation(createConversationDto);
            var message = new SendMessageRequest(RandomString(), "Hello", participant1);
            await _chatServiceClient.SendMessage(conversationDto.Id, message);

            try
            {
                await _chatServiceClient.SendMessage(conversationDto.Id, message);
                Assert.Fail("An exception was expected but was not thrown");
            }
            catch (ChatServiceException e)
            {
                Assert.AreEqual(HttpStatusCode.Conflict, e.StatusCode);
            }
        }

        private static string RandomString()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
