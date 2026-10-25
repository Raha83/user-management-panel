using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ManagementPanel
{
    public class ManagementContext : IdentityDbContext<User>
    {
        public ManagementContext(DbContextOptions<ManagementContext> options) : base(options)
        {

        }
        public DbSet<Permission> permissions { get; set; }
    }
}