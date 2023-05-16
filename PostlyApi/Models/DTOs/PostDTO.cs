namespace PostlyApi.Models.DTOs
{
    public class PostDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public AuthorDTO Author { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpvoteCount { get; set; }
        public int DownvoteCount { get; set; }
    }
}
