namespace PostlyApi.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public User Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; }

        /// <summary>
        /// Empty constructor for EF.
        /// </summary>
        public Comment()
        {
        }

        public Comment(User author, DateTime createdAt, string content)
        {
            Author = author;
            CreatedAt = createdAt;
            Content = content;
        }
    }
}
