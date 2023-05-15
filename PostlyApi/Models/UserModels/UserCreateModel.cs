using PostlyApi.Enums;

namespace PostlyApi.Models.UserDataModels
{
    // Model sent by the client to create a new user

    public class UserCreateModel
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public Role? Role { get; set; }
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? Birthday { get; set; }
        public Gender? Gender { get; set; }
    }
}
