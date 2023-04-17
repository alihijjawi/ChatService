namespace ChatService.DataContracts
{
    public class UserProfileDto
    {
        public UserProfileDto(string username, string firstName, string lastName, string profilePictureId)
        {
            Username = username;
            FirstName = firstName;
            LastName = lastName;
            ProfilePictureId = profilePictureId;
        }

        public string Username { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string ProfilePictureId { get; }
    }
}
