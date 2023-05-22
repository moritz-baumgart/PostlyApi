namespace PostlyApi.Models.DTOs
{
    public class PostDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public AuthorDTO Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UpvoteCount { get; set; }
        public int DownvoteCount { get; set; }
        public int CommentCount { get; set; }
        public VoteInteractionType? Vote { get; set; }
        public bool? HasCommented { get; set; }
    }
}
