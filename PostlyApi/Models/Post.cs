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
    }
}
