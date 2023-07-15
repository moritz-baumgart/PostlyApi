using System.ComponentModel.DataAnnotations;

namespace PostlyApi.Models.Requests
{
    /// <summary>
    /// Request sent by a client to create a comment
    /// </summary>
    public class CommentCreateRequest
    {
        public int PostId { get; set; }
        public string CommentContent { get; set; }
    }
}
