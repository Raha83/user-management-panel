using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Identity.Client;

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
                    return NotFound("کاربر مورد نظر یافت نشد");
                }
                else
                {
                    var result = await _userManager.ConfirmEmailAsync(user, token);
                    if (!result.Succeeded)
                    {
                        return StatusCode(500, "لینک فعالسازی درست نیست یا منقضی شده است");
                    }
                    user.IsActive = true;
                    var updatedUser = await _userManager.UpdateAsync(user);
                    if (!updatedUser.Succeeded)
                    {
                        return StatusCode(500, "فعالسازی حساب کاربری با خطا مواجه شد");
                    }
                    return Page();
                }
            }
        }
    }
}