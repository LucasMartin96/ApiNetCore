using System.ComponentModel.DataAnnotations;

namespace FirstApi2xd.Contracts.v1.Requests
{
    public class UserLoginRequest
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}