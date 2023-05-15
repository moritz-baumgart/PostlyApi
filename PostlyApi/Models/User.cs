using PostlyApi.Utilities;
using System.Security.Cryptography;
using System.Text;

namespace PostlyApi.Models
{
    public class User
    {
        public long Id { get; set; }
        public String Username { get; set; }
        public String? DisplayName { get; set; }
        public byte[] PasswordHash { get; set; }
        public String? Email { get; set; }
        public String? PhoneNumber { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime CreatedAt { get; set; }
        public Gender? Gender { get; set; }
        public Role Role { get; set; }
        public ICollection<User> Follower { get; set; }
        public ICollection<User> Following { get; set; }
        public ICollection<Post> Posts { get; set; }
        public ICollection<Post> UpvotedPosts { get; set; }
        public ICollection<Post> DownvotedPosts { get; set; }

        /// <summary>
        /// Creates a new user with given username and password.
        /// </summary>
        /// <param name="username">The username the new user should have.</param>
        /// <param name="password">The password the new user should have, it will be hashed using HMACSHA512.</param>
        public User(String username, String password) : this(username, PasswordUtilities.ComputePasswordHash(password)) { }

        /// <summary>
        /// Creates a new user with given username and password.
        /// </summary>
        /// <param name="username">The username the new user should have.</param>
        /// <param name="password">The password the new user should have, it will be hashed using HMACSHA512.</param>
        /// <param name="role">The role the new user should have.</param>
        public User(String username, String password, Role role) : this(username, PasswordUtilities.ComputePasswordHash(password), role) { }

        /// <summary>
        /// Creates a new user with given username and password and a default role of "User".
        /// </summary>
        /// <param name="username"></param>
        /// <param name="passwordHash"></param>
        public User(String username, byte[] passwordHash) : this(username, passwordHash, Role.User) { }

        /// <summary>
        /// Creates a new user with given username, password and role.
        /// The "CreatedAt" is initialized with a current timestamp.
        /// </summary>
        /// <param name="username">The username the new user should have.</param>
        /// <param name="passwordHash">The password the new user should have, it should be hashed using HMACSHA512.</param>
        /// <param name="role">The role the new user should have.</param>
        public User(String username, byte[] passwordHash, Role role)
        {
            Username = username;
            PasswordHash = passwordHash;
            Role = role;
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
