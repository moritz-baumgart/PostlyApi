using PostlyApi.Enums;

namespace PostlyApi.Models.DTOs
{
    public class UserDataViewModel
    {
        // public data: 
        public long Id { get; set; } // unchangeable
        public DateTimeOffset CreatedAt { get; set; } // unchangeable 
        public string Username { get; set; }
        public string? DisplayName { get; set; }
        public Role Role { get; set; }
        public DateTimeOffset? Birthday { get; set; }
        public Gender? Gender { get; set; }
        public byte[]? ProfilePicture { get; set; }

        // private data:
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
