using System.Security;
using Microsoft.AspNetCore.Identity;

namespace ManagementPanel
{
    public class User
    {
        public int id { get; set; }
        public string? fname { get; set; }
        public string? lname { get; set; }
        public string email { get; set; } = null!;
        public string? profileImage { get; set; }
        public bool isActive { get; set; }
        public virtual ICollection<Role> roles { get; set; } = null!;
        public virtual ICollection<Permission> permissions { get; set; } = null!;
    }
}