namespace RoleBasedApplication.Models
{
    public class RegisterResponseDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; }
    }
}
