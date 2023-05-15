namespace PostlyApi.Models.PostModels
{
    // Model sent by the client to create a new post

    public class PostCreateModel
    {
        public string Content { get; set; }
        public long AuthorId { get; set; }
    }
}
