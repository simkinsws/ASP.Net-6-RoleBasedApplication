using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RoleBasedApplication.Entities
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;

        [JsonIgnore]
        public byte[] PasswordHash { get; set; }
        [JsonIgnore]
        public byte[] PasswordSalt { get; set; }
        [RegularExpression("User|Admin", ErrorMessage = "Invalid Role")]
        public string Role { get; set; }
        public List<PostModel> Posts { get; set; }
    }
}
