using PostlyApi.Enums;

namespace PostlyApi.Models.UserDataModels
{
    // Model sent to the client to view user data

    public class UserViewModel
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Username { get; set; }
        public Role Role { get; set; }
        public string DisplayName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? Birthday { get; set; }
        public Gender? Gender { get; set; }
    }
}
