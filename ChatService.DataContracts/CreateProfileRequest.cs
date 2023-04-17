namespace ChatService.DataContracts
{
    public class CreateProfileRequest
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureId { get; set; }
    }
}
