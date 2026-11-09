using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManagementPanel.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        public ResetPasswordModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public ResetInput Input { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? UserId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Token { get; set; }
        public class ResetInput
        {
            [Display(Name = "New Password")]
            [Required(ErrorMessage = "Please enter your new password")]
            [DataType(DataType.Password)]
            public string? Password { get; set; }

            [Display(Name = "Confirm Password")]
            [Required(ErrorMessage = "Please repeat your password")]
            [DataType(DataType.Password)]
            [Compare(nameof(Password), ErrorMessage = "Password does not match with the repeat of it")]
            public string? ConfirmPassword { get; set; }
        }
        public async Task<IActionResult> OnGetAsync(string token, string userId)
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
                    NotFound("User not found");
                }
                else
                {
                    return Page();
                }
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            else
            {
                var user = await _userManager.FindByIdAsync(UserId);
                if (user is null)
                {
                    NotFound("User not found");
                }
                else
                {
                    var result = await _userManager.ResetPasswordAsync(
                 user,
                 Token!,
                 Input.Password!
             );

                    if (result.Succeeded)
                    {
                        return RedirectToPage("/Account/Login");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return Page();
                }
                return Page();
            }
        }
    }
}