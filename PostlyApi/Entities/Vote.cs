using PostlyApi.Enums;

namespace PostlyApi.Entities
{
    public class Vote
    {
        public long UserId { get; set; }
        public User User { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        public VoteType VoteType { get; set; }
    }
}
