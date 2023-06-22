namespace PostlyApi.Entities
{
    public class Login
    {
        public DateTimeOffset CreatedAt { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
    }
}
