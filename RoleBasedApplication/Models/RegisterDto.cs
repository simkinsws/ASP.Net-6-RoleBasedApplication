using System.ComponentModel.DataAnnotations;

namespace RoleBasedApplication.Models
{
    public class RegisterDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        //[RegularExpression("User|Admin", ErrorMessage = "Invalid Role")]
        //public string Role { get; set; }
    }
}
