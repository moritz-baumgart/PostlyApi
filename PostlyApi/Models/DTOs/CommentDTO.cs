namespace PostlyApi.Models.DTOs
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public AuthorDTO Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; }
    }
}
