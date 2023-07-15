namespace PostlyApi.Models.DTOs
{
    /// <summary>
    /// User information sent by the server
    /// </summary>
    public class UserDTO
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string? DisplayName { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
