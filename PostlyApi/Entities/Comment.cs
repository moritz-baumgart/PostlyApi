using PostlyApi.Models.DTOs;

namespace PostlyApi.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public User Author { get; set; }
        public int PostId { get; set; }
        public Post CommentedPost { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; }

        /// <summary>
        /// Empty constructor for EF.
        /// </summary>
        public Comment()
        {
        }

        public Comment(User author, Post post, string content)
            : this(author, post, content, DateTime.UtcNow) { }

        public Comment(User author, Post post, string content, DateTime createdAt)
        {
            Author = author;
            CommentedPost = post;
            CreatedAt = createdAt;
            Content = content;
        }
    }
}
