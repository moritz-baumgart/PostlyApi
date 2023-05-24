using System.ComponentModel.DataAnnotations;

namespace PostlyApi.Models.Requests
{
    public class InteractionRequest
    {
        public int PostId { get; set; }
        public VoteInteractionType Type { get; set; }
    }
}
