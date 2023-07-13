namespace PostlyApi.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public long UserId { get; set; } 
        public User Author { get; set; } 
        public DateTimeOffset CreatedAt { get; set; }
        public Guid? ImageId { get; set; }
        public Image? AttachedImage { get; set; }
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        /// <summary>
        /// Empty constructor for EF.
        /// </summary>
        public Post()
        {
        }

        public Post(string content, User author, DateTimeOffset createdAt)
        {
            Content = content;
            Author = author;
            CreatedAt = createdAt;
        }
    }
}
