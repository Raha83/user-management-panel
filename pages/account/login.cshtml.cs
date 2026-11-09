using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManagementPanel.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signinManager;

        public LoginModel(UserManager<User> userManager, SignInManager<User> signinManager)
        {
            _userManager = userManager;
            _signinManager = signinManager;
        }

        [BindProperty]
        public LoginInput Input { get; set; }
        public class LoginInput
        {
            [Required(ErrorMessage = "please enter your email")]
            [Display(Name = "Email")]
            [EmailAddress(ErrorMessage = "email format is incorrect")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "please enter your password")]
            [Display(Name = "Password")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "No user found with the provided information");
                return Page();
            }
            else
            {
                var passwordCheck = await _signinManager.PasswordSignInAsync(user, Input.Password, isPersistent: false, lockoutOnFailure: false);

                if (passwordCheck.Succeeded)
                {
                    return RedirectToPage("/Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Given information is incorrect");
                    return Page();
                }
            }
        }
    }
}