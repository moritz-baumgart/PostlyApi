using PostlyApi.Enums;

namespace PostlyApi.Models.DTOs
{
    /// <summary>
    /// Returned by the server when updating a vote
    /// </summary>
    public class VoteUpdateViewModel
    {
        public int PostId { get; set; } // the post that had a vote update
        public int UpvoteCount { get; set; } // the upvote count after the vote update
        public int DownvoteCount { get; set; } // the downvote count after the vote update
        public long UserId { get; set; } // the user who updated their vote
        public VoteType? VoteType { get; set; } // the type of vote the user set
    }
}
