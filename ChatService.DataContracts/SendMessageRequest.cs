using System.Collections.Generic;

namespace ChatService.DataContracts
{
    public class SendMessageRequest
    {
        public SendMessageRequest()
        {
        }
        
        public SendMessageRequest(string id, string text, string senderUsername)
        {
            Id = id;
            Text = text;
            SenderUsername = senderUsername;
        }

        public string Id { get; set; }
        public string Text { get; set; }
        public string SenderUsername { get; set; }

        public override bool Equals(object obj)
        {
            var dto = obj as SendMessageRequest;
            return dto != null &&
                   Text == dto.Text &&
                   SenderUsername == dto.SenderUsername;
        }

        public override int GetHashCode()
        {
            var hashCode = 928530866;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Text);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SenderUsername);
            return hashCode;
        }
    }
}
