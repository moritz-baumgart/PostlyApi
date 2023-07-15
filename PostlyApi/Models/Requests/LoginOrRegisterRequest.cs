using PostlyApi.Enums;

namespace PostlyApi.Models.Requests
{
    /// <summary>
    /// Request sent by a client to login an existing user or register a new one
    /// </summary>
    public class LoginOrRegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Role? Role { get; set; }
    }
}
