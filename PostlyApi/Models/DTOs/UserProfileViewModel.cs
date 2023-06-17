using PostlyApi.Enums;

namespace PostlyApi.Models.DTOs
{
    public class UserProfileViewModel
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public Role Role { get; set; }
        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }
        public DateTime? Birthday { get; set; }
        public Gender? Gender { get; set; }
    }
}
