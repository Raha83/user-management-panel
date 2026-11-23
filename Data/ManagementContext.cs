using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ManagementPanel.Models;

namespace ManagementPanel.Data
{
    public class ManagementContext : IdentityDbContext<User>
    {
        public ManagementContext(DbContextOptions<ManagementContext> options) : base(options)
        {

        }
        public DbSet<Permission> permissions { get; set; }
    }
}