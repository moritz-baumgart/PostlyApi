namespace PostlyApi.Models.PostModels
{
    // Model sent to the client to view a post

    public class PostViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Content { get; set; }
        public long UserId { get; set; }
    }
}
