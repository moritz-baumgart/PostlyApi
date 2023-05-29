using PostlyApi.Enums;

namespace PostlyApi.Models.Requests
{
    public class ProfileUpdateRequest
    {
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? Birthday { get; set; }
        public Gender? Gender { get; set; }
    }
}
