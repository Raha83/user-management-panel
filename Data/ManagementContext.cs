using Microsoft.EntityFrameworkCore;

namespace ManagementPanel
{
    public class ManagementContext : DbContext
    {
        public ManagementContext(DbContextOptions<ManagementContext> options) : base(options)
        {

        }
        public DbSet<User> users { get; set; }
        public DbSet<Role> roles { get; set; }
        public DbSet<Permission> permissions { get; set; }
    }
}