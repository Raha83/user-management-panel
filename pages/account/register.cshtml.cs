using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Encodings.Web;
using ManagementPanel.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ManagementPanel.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly EmailSender _emailSender;
        public string? message { get; set; }
        public RegisterModel(UserManager<User> userManager, EmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public RegisterInput Input { get; set; } = new();
        public class RegisterInput
        {
            [Display(Name = "First Name")]
            public string? FName { get; set; }

            [Display(Name = "Last Name")]
            public string? LName { get; set; }

            [Required(ErrorMessage = "Please enter your email")]
            [Display(Name = "Email")]
            [EmailAddress(ErrorMessage = "Email format is incorrect")]
            public string Email { get; set; } = string.Empty;

            [Display(Name = "Phone Number")]
            [DataType(DataType.PhoneNumber)]
            [RegularExpression(@"^[0-9]{11}", ErrorMessage = "Phone number should contain 11 digits")]
            public string? PhoneNumber { get; set; }

            [Required(ErrorMessage = "Please enter your password")]
            [Display(Name = "Password")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "Please repeat your password")]
            [Display(Name = "Confirm Password")]
            [DataType(DataType.Password)]
            [Compare(nameof(Password), ErrorMessage = "Password does not match with the repeat of it")]
            public string ConfirmPassword { get; set; } = string.Empty;

            [Display(Name = "Profile Image")]
            [DataType(DataType.ImageUrl, ErrorMessage = "File format is incorrect")]
            public IFormFile? ProfileImage { get; set; }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var checkEmail = await _userManager.FindByEmailAsync(Input.Email);
            if (checkEmail is not null)
            {
                if (checkEmail.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "This user already exists in the system");
                    return Page();
                }
                else
                {
                    await SendConfirmationEmail(checkEmail);
                    return RedirectToPage("/Account/Confirmation", new { message = "activation", userId = checkEmail.Id });
                }
            }
            else
            {
                string? imagePath = null;
                if (Input.ProfileImage is { Length: > 0 })
                {
                    var allowedExtensions = new[] { ".jpg", ".png", ".jpeg", ".webp" };
                    var extension = Path.GetExtension(Input.ProfileImage.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError(string.Empty, "Only image files are allowed");
                        return Page();
                    }
                    var fileName = Guid.NewGuid() + Path.GetExtension(Input.ProfileImage.FileName);
                    var path = Path.Combine("wwwroot/uploads/profiles", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                    using var stream = new FileStream(path, FileMode.Create);
                    await Input.ProfileImage.CopyToAsync(stream);
                    imagePath = $"/uploads/profiles/{fileName}";
                }

                var user = new User
                {
                    FName = Input.FName,
                    LName = Input.LName,
                    UserName = Input.Email,
                    Email = Input.Email,
                    PhoneNumber = Input.PhoneNumber,
                    ProfileImage = imagePath,
                    IsActive = false
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddClaimsAsync(user, new List<Claim>
                {
                    new Claim("FName",user.FName ?? ""),
                    new Claim("LName",user.LName ?? ""),
                    new Claim("Profile",user.ProfileImage ?? "")
                });

                    await SendConfirmationEmail(user);
                    return RedirectToPage("/Account/Confirmation", new { message = "activation", userId = user.Id });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
        private async Task SendConfirmationEmail(User user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Page("/Account/Activate",
            pageHandler: null,
            values: new
            {
                userId = user.Id,
                token
            },
            protocol: Request.Scheme
            );
            var emailBody = $"""
                <h2>Account Activation</h2>
                <p>Please click on the link below for your account activation</p>
                <a href="{HtmlEncoder.Default.Encode(confirmationLink!)}">Activation Link</a>
                """;
            await _emailSender.SendEmailAsync(user.Email!,
            "Account Activation",
            emailBody);
        }
    }
}
