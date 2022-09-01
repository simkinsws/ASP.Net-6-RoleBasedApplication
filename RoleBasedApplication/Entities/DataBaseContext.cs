using Microsoft.EntityFrameworkCore;

namespace RoleBasedApplication.Entities
{
    public class DataBaseContext : DbContext
    {
        protected readonly IConfiguration _configuration;

        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {
           
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //{
        //    // connect to sql server with connection string from app settings
        //    options.UseSqlServer(_configuration.GetConnectionString("Database"));
        //}

        public DbSet<UserModel> Users { get; set; }
        public DbSet<PostModel> Posts { get; set; }
    }
}
