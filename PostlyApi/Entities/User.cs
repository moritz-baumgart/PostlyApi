using PostlyApi.Enums;
using PostlyApi.Utilities;

namespace PostlyApi.Entities
{
    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public byte[] PasswordHash { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime CreatedAt { get; set; }
        public Gender? Gender { get; set; }
        public Role Role { get; set; }
        public ICollection<User> Follower { get; set; } = new List<User>();
        public ICollection<User> Following { get; set; } = new List<User>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Post> UpvotedPosts { get; set; } = new List<Post>();
        public ICollection<Post> DownvotedPosts { get; set; } = new List<Post>();

        /// <summary>
        /// Creates a new user with given username and password.
        /// </summary>
        /// <param name="username">The username the new user should have.</param>
        /// <param name="password">The password the new user should have, it will be hashed using HMACSHA512.</param>
        /// <param name="role">The role the new user should have.</param>
        public User(string username, string password, Role role = Role.User) : this(username, PasswordUtilities.ComputePasswordHash(password), role) { }

        /// <summary>
        /// Creates a new user with given username, password and role.
        /// The "CreatedAt" is initialized with a current timestamp.
        /// </summary>
        /// <param name="username">The username the new user should have.</param>
        /// <param name="passwordHash">The password the new user should have, it should be hashed using HMACSHA512.</param>
        /// <param name="role">The role the new user should have.</param>
        public User(string username, byte[] passwordHash, Role role = Role.User)
        {
            Username = username;
            DisplayName = username;
            PasswordHash = passwordHash;
            Role = role;
            CreatedAt = DateTime.UtcNow;
        }
    }

}
