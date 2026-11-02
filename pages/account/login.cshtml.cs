using System.ComponentModel.DataAnnotations;
using Azure;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace ManagementPanel.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signinManager;
        private readonly IConfiguration _config;

        public LoginModel(IConfiguration config, UserManager<User> userManager, SignInManager<User> signinManager)
        {
            _config = config;
            _userManager = userManager;
            _signinManager = signinManager;
        }

        [BindProperty]
        public LoginInput Input { get; set; }
        public class LoginInput
        {
            [Required(ErrorMessage = "لطفا ایمیل خود را وارد کنید.")]
            [Display(Name = "ایمیل")]
            [EmailAddress(ErrorMessage = "فرمت ایمیل وارد شده صحیح نیست.")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "لطفا رمز عبور خود را وارد کنید.")]
            [Display(Name = "رمز عبور")]
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
                return Page();
            }
            else
            {
                var passwordCheck = await _signinManager.PasswordSignInAsync(user, Input.Password, isPersistent: false, lockoutOnFailure: false);

                if (passwordCheck.Succeeded)
                {
                    return RedirectToPage("/index");
                }
                else
                {
                    return Page();
                }
            }
        }
    }
}