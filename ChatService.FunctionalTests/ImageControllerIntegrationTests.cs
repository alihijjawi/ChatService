using ChatService.Client;
using ChatService.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChatServiceFunctionalTests
{
    [TestClass]
    [TestCategory("Integration")]
    [TestCategory("Functional")]
    public class ImageControllerIntegrationTests
    {
        private ChatServiceClient _chatServiceClient;

        [TestInitialize]
        public void TestInitialize()
        {
            _chatServiceClient = TestUtils.CreateChatServiceClient();
        }

        [TestMethod]
        public async Task UploadDownloadDeleteImage()
        {
            string str = Guid.NewGuid().ToString();
            var bytes = Encoding.UTF8.GetBytes(str);
            var stream = new MemoryStream(bytes);

            UploadImageResponse uploadImageResponse = await _chatServiceClient.UploadImage(stream);

            DownloadImageResponse downloadImageResponse =
                await _chatServiceClient.DownloadImage(uploadImageResponse.ImageId);

            Assert.AreEqual(str, Encoding.UTF8.GetString(downloadImageResponse.ImageData));

            await _chatServiceClient.DeleteImage(uploadImageResponse.ImageId);
            try
            {
                await _chatServiceClient.DownloadImage(uploadImageResponse.ImageId);
            }
            catch (ChatServiceException e)
            {
                Assert.AreEqual(HttpStatusCode.NotFound, e.StatusCode);
            }
        }
    }
}
