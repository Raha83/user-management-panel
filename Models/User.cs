using System.Security;
using Microsoft.AspNetCore.Identity;

namespace ManagementPanel
{
    public class User : IdentityUser
    {
        public string? fname { get; set; }
        public string? lname { get; set; }
        public string? profileImage { get; set; }
        public bool isActive { get; set; }
        public virtual ICollection<Role> roles { get; set; } = null!;
    }
}