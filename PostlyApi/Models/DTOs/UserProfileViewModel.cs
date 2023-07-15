using PostlyApi.Enums;

namespace PostlyApi.Models.DTOs
{
    /// <summary>
    /// Profile information returned by the server
    /// </summary>
    public class UserProfileViewModel
    {
        public long Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string Username { get; set; }
        public string? DisplayName { get; set; }
        public Role Role { get; set; }
        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }
        public DateTimeOffset? Birthday { get; set; }
        public Gender? Gender { get; set; }
        public string? ProfileImageUrl { get; set; }
        public bool? Follow { get; set; }
    }
}
