using Microsoft.AspNetCore.Identity;

namespace ManagementPanel.Models
{
    public class User : IdentityUser
    {
        public string? FName { get; set; }
        public string? LName { get; set; }
        public string? ProfileImage { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Role> Roles { get; set; } = null!;
    }
}