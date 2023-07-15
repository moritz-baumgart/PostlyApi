namespace PostlyApi.Models.Requests
{
    /// <summary>
    /// Request sent by a client to update a password
    /// </summary>
    public class PasswordUpdateRequest
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
