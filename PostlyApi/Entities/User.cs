using PostlyApi.Enums;

namespace PostlyApi.Entities
{
    public class User
    {
        // meta data (unchangeable):
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }

        // account data:
        public string Username { get; set; }
        public string PasswordHash { get; set; }    // not public
        public Role Role { get; set; }

        // profile data:
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? Birthday { get; set; }
        public Gender? Gender { get; set; }

        // relations:
        public ICollection<User>? Follower { get; set; }
        public ICollection<User>? Following { get; set; }
        public ICollection<Post>? Posts { get; set; }
        public ICollection<Post>? UpvotedPosts { get; set; }
        public ICollection<Post>? DownvotedPosts { get; set; }

        public User(string username, string passwordHash, Role role=Role.User)
        {
            Username = username;
            PasswordHash = passwordHash;
            Role = role;
            CreatedAt = DateTime.Now;
        }

    }
}
