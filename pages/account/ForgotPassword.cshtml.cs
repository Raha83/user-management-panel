using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using ManagementPanel.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManagementPanel.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly EmailSender _emailSender;
        public string? message { get; set; }
        public ForgotPasswordModel(UserManager<User> userManager, EmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }
        [BindProperty]
        public ForgotInput Input { get; set; } = new();
        public class ForgotInput
        {
            [Display(Name = "Email")]
            [Required(ErrorMessage = "Please enter your email")]
            [EmailAddress(ErrorMessage = "Email format is incorrect")]
            public string? Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            else
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user is null)
                {
                    ModelState.AddModelError(string.Empty, "This user is not registered");
                }
                else
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetLink = Url.Page("/Account/ResetPassword",
                    pageHandler: null,
                    values: new
                    {
                        userId = user.Id,
                        token
                    },
                    protocol: Request.Scheme);

                    var emailBody = $"""
                <h2>Password Reset</h2>
                <p>Please click on the link below for resetting your password</p>
                <a href="{HtmlEncoder.Default.Encode(resetLink!)}">Reset Password Link</a>
                """;
                    await _emailSender.SendEmailAsync(user.Email, "Password Reset", emailBody);
                    return RedirectToPage("/Account/Confirmation", new { message = "reset" });

                }
                return Page();
            }
        }
    }
}