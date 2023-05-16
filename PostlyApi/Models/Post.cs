namespace PostlyApi.Models
{
    public class Post
    {
        public int Id { get; set; }
        public String Content { get; set; }
        public long UserId { get; set; }
        public User Author { get; set; }
        public DateTime CreatedDate { get; set; }
        public ICollection<User> UpvotedBy { get; set; }
        public ICollection<User> DownvotedBy { get; set; }

        /// <summary>
        /// Empty constructor for EF.
        /// </summary>
        public Post()
        {
        }

        public Post(string content, User author, DateTime createdDate)
        {
            Content = content;
            Author = author;
            CreatedDate = createdDate;
        }
    }
}
