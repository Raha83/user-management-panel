using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ManagementPanel.Models;

namespace ManagementPanel.Pages.Dashboard
{
    [Authorize(Roles = "Admin")]
    public class UserListModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        public UserListModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public List<User> users { get; set; } = new();
        public async Task<IActionResult> OnGetAsync()
        {
            users = await _userManager.Users.ToListAsync();

            return Page();
        }
    }
}