using System.ComponentModel.DataAnnotations;
using PostlyApi.Enums;

namespace PostlyApi.Models.Requests
{
    public class InteractionRequest
    {
        public int PostId { get; set; }
        public VoteInteractionType Type { get; set; }
    }
}
