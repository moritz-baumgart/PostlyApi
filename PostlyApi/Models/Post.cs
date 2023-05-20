namespace PostlyApi.Models
{
    public class Post
    {
        public int Id { get; set; }
        public String Content { get; set; }
        public long UserId { get; set; }
        public User Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<User> UpvotedBy { get; set; }
        public ICollection<User> DownvotedBy { get; set; }
        public ICollection<Comment> Comments { get; set; }

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
