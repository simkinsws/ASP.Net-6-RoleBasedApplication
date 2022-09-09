namespace RoleBasedApplication.Models
{
    public class LoginResponseDto
    {
        public string token { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
    }
}
