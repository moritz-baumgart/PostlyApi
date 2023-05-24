using System.ComponentModel.DataAnnotations;

namespace PostlyApi.Models.Requests
{
    public class CommentRequest
    {
        public int PostId { get; set; }
        public string CommentContent { get; set; }
    }
}
