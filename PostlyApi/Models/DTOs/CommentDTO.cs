namespace PostlyApi.Models.DTOs
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public UserDTO Author { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string Content { get; set; }
    }
}
