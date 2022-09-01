using RoleBasedApplication.Entities;

namespace RoleBasedApplication.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } 
        public string Role { get; set; }
        public List<PostModel> Posts { get; set; }
    }
}
