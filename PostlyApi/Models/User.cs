namespace PostlyApi.Models
{
    public class User
    {
        public long Id { get; set; }
        public String Username { get; set; }
        public String? DisplayName { get; set; }
        public String PasswordHash { get; set; }
        public String? Email { get; set; }
        public String? PhoneNumber { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime CreatedAt { get; set; }
        public Gender? Gender { get; set; }
        public ICollection<User> Follower { get; set; }
        public ICollection<User> Following { get; set; }
        public ICollection<Post> Posts { get; set; }
        public ICollection<Post> UpvotedPosts { get; set; }
        public ICollection<Post> DownvotedPosts { get; set; }

        public User(String username, String passwordHash)
        {
            Username = username;
            PasswordHash = passwordHash;
            CreatedAt = DateTime.Now;
        }

    }

    public enum Gender
    {
        Male,
        Female,
        Other
    }

    public enum Role
    {
        User,
        Moderator,
        Admin
    }

}
