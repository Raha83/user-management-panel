using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ManagementPanel.Models;

namespace ManagementPanel.Pages.Account
{
    public class ConfirmationModel : PageModel
    {
        public string? Message { get; set; }
        private readonly UserManager<User> _userManager;
        public ConfirmationModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> OnGetAsync(string message, string userId)
        {
            if (string.IsNullOrEmpty(message) || string.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }
            else
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user is null)
                {
                    return NotFound("User not found");
                }
                else
                {
                    Message = message;
                }

                return Page();
            }
        }
    }
}