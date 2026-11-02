using Microsoft.AspNetCore.Identity;

namespace ManagementPanel
{
    public class Role : IdentityRole
    {
        public string? Description { get; set; }
        public virtual ICollection<Permission> permissions { get; set; } = null!;
    }
}