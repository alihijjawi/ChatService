using Newtonsoft.Json;

namespace ChatService.DataContracts
{
    public class DownloadImageResponse
    {
        public DownloadImageResponse(byte[] imageData)
        {
            ImageData = imageData;
        }

        [JsonRequired]
        [JsonProperty(nameof(ImageData))]
        public byte[] ImageData { get; }
    }
}
