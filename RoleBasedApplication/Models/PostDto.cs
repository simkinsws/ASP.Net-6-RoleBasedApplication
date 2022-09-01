namespace RoleBasedApplication.Models
{
    public class PostDto
    {
        public int Id { get; set; }
        public string Phone { get; set; }
        public string Branch { get; set; }
        public string Description { get; set; }
        public string Solution { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
    }
}