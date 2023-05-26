using PostlyApi.Models.DTOs;

namespace PostlyApi.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public long UserId { get; set; }
        public User Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<User> UpvotedBy { get; set; } = new List<User>();
        public ICollection<User> DownvotedBy { get; set; } = new List<User>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        /// <summary>
        /// Empty constructor for EF.
        /// </summary>
        public Post()
        {
        }

        public Post(string content, User author, DateTime createdAt)
        {
            Content = content;
            Author = author;
            CreatedAt = createdAt;
        }
    }
}
