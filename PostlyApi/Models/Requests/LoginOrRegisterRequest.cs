using PostlyApi.Enums;

namespace PostlyApi.Models.Requests
{
    public class LoginOrRegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Role? Role { get; set; }
    }
}
