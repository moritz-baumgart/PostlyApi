using PostlyApi.Enums;

namespace PostlyApi.Models.Requests
{
    public class UserDataUpdateRequest
    {
        // public data: 
        public string? Username { get; set; }
        public string? DisplayName { get; set; }
        public Role? Role { get; set; }

        // private data:
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? Birthday { get; set; }
        public Gender? Gender { get; set; }
    }
}
