using System.Net;
using ChatService.Client;
using ChatService.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChatServiceFunctionalTests
{
    /// <summary>
    /// The integration tests are used to validate the full API execution end-to-end.
    /// These are usually the tests that allow us to find most issues and they tend to
    /// be less fragile because they are decoupled from implementation details (they rely only
    /// on the document API).
    /// </summary>
    [TestClass]
    [TestCategory("Integration")]
    [TestCategory("Functional")]
    public class ProfileControllerIntegrationTests
    {
        private ChatServiceClient _chatServiceClient;

        [TestInitialize]
        public void TestInitialize()
        {
            _chatServiceClient = TestUtils.CreateChatServiceClient();
        }

        [TestMethod]
        public async Task CreateGetProfile()
        {
            var createProfileDto = new CreateProfileRequest
            {
                Username = Guid.NewGuid().ToString(),
                FirstName = "Nehme",
                LastName = "Bilal",
                ProfilePictureId = Guid.NewGuid().ToString()
            };

            await _chatServiceClient.CreateProfile(createProfileDto);
            UserProfileDto userProfile = await _chatServiceClient.GetProfile(createProfileDto.Username);

            Assert.AreEqual(createProfileDto.Username, userProfile.Username);
            Assert.AreEqual(createProfileDto.FirstName, userProfile.FirstName);
            Assert.AreEqual(createProfileDto.LastName, userProfile.LastName);
            Assert.AreEqual(createProfileDto.ProfilePictureId, userProfile.ProfilePictureId);
        }

        [TestMethod]
        public async Task GetNonExistingProfile()
        {
            try
            {
                await _chatServiceClient.GetProfile(Guid.NewGuid().ToString());
                Assert.Fail("A ChatServiceException was expected but was not thrown");
            }
            catch (ChatServiceException e)
            {
                Assert.AreEqual(HttpStatusCode.NotFound, e.StatusCode);
            }
        }

        [TestMethod]
        public async Task CreateDuplicateProfile()
        {
            var createProfileDto = new CreateProfileRequest
            {
                Username = Guid.NewGuid().ToString(),
                FirstName = "Nehme",
                LastName = "Bilal"
            };

            await _chatServiceClient.CreateProfile(createProfileDto);

            try
            {
                await _chatServiceClient.CreateProfile(createProfileDto);
                Assert.Fail("A ChatServiceException was expected but was not thrown");
            }
            catch (ChatServiceException e)
            {
                Assert.AreEqual(HttpStatusCode.Conflict, e.StatusCode);
            }
        }

        [TestMethod]
        [DataRow("", "Nehme", "Bilal")]
        [DataRow(null, "Nehme", "Bilal")]
        [DataRow("nbilal", "", "Bilal")]
        [DataRow("nbilal", null, "Bilal")]
        [DataRow("nbilal", "Nehme", "")]
        [DataRow("nbilal", "Nehme", null)]
        public async Task CreateProfile_InvalidDto(string username, string firstName, string lastName)
        {
            var createProfileDto = new CreateProfileRequest
            {
                Username = username,
                FirstName = firstName, 
                LastName = lastName
            };

            try
            {
                await _chatServiceClient.CreateProfile(createProfileDto);
                Assert.Fail("A ChatServiceException was expected but was not thrown");
            }
            catch (ChatServiceException e)
            {
                Assert.AreEqual(HttpStatusCode.BadRequest, e.StatusCode);
            }
        }
    }
}
