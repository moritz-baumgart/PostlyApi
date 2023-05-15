namespace PostlyApi.Entities
{
    public class Post
    {
        // meta data (unchangable):
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }

        // content data:
        public string Content { get; set; }

        // relations:
        public long UserId { get; set; }
        public User Author { get; set; }
        public ICollection<User>? UpvotedBy { get; set; }
        public ICollection<User>? DownvotedBy { get; set; }

        public Post(string content, long authorId)
        {
            // TODO: check for validity
            Content = content;
            UserId = authorId;
        }
    }
}
