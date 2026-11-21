using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManagementPanel.Pages.Account
{
    public class ActivateModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        public ActivateModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> OnGetAsync(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
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
                    var result = await _userManager.ConfirmEmailAsync(user, token);
                    if (!result.Succeeded)
                    {
                        return StatusCode(400, "Activation link is incorrect or expired");
                    }

                    user.IsActive = true;
                    var updatedUser = await _userManager.UpdateAsync(user);
                    if (!updatedUser.Succeeded)
                    {
                        return StatusCode(500, "Account activation faced error");
                    }
                    return Page();
                }
            }
        }
    }
}