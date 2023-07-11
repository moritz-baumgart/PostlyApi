namespace PostlyApi.Models.DTOs
{
    public class UserDTO
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string? DisplayName { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
