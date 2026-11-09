using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using ManagementPanel.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
            [Phone(ErrorMessage = "Phone number format is incorrect")]
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

            string? imagePath = null;
            if (Input.ProfileImage is { Length: > 0 })
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(Input.ProfileImage.FileName);
                var path = Path.Combine("wwwroot/uploads/profiles", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                using var stream = new FileStream(path, FileMode.Create);
                await Input.ProfileImage.CopyToAsync(stream);
                imagePath = $"/uploads/profiles/{fileName}";
            }

            var checkEmail = await _userManager.FindByEmailAsync(Input.Email);
            if (checkEmail is not null)
            {
                ModelState.AddModelError(string.Empty, "This user already exists in the system");
            }
            else
            {
                var user = new User
                {
                    FName = Input.FName,
                    LName = Input.LName,
                    UserName = Input.Email,
                    Email = Input.Email,
                    PhoneNumber = Input.PhoneNumber,
                    PasswordHash = Input.Password,
                    ProfileImage = imagePath,
                    IsActive = false
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
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
                    await _emailSender.SendEmailAsync(user.Email,
                    "Account Activation",
                    emailBody);
                    return RedirectToPage("/Account/Confirmation", new { message = "activation" });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return Page();
        }
    }
}
