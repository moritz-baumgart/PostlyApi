using System.ComponentModel.DataAnnotations;

namespace PostlyApi.Models.Requests
{
    public class CommentCreateRequest
    {
        public int PostId { get; set; }
        public string CommentContent { get; set; }
    }
}
