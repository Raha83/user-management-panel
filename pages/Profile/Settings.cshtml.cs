using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManagementPanel.Pages.Profile
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signinManager;
        private readonly IWebHostEnvironment _environment;
        public ProfileModel(UserManager<User> userManager, SignInManager<User> signInManager, IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _signinManager = signInManager;
            _environment = environment;
        }

        [BindProperty]
        public ProfileInput Input { get; set; }
        public class ProfileInput
        {
            public string? CurrentImage { get; set; }

            public IFormFile? NewImage { get; set; }

            [Display(Name = "First Name")]
            public string? FName { get; set; }

            [Display(Name = "Last Name")]
            public string? LName { get; set; }

            [Display(Name = "Email")]
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Display(Name = "Phone Number")]
            [DataType(DataType.PhoneNumber)]
            [RegularExpression(@"^[0-9]{11}", ErrorMessage = "Phone number should contain 11 digits")]
            public string? PhoneNumber { get; set; }

            [Display(Name = "Current Password")]
            [Required(ErrorMessage = "Please enter your password")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound("User not found");
            }
            else
            {
                Input = new ProfileInput
                {
                    CurrentImage = user.ProfileImage,
                    FName = user.FName,
                    LName = user.LName,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber
                };
                return Page();
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadCurrentImage();
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                if (user is null)
                {
                    return NotFound("User not found");
                }
                else
                {
                    var checkPassword = await _userManager.CheckPasswordAsync(user, Input.Password);
                    if (!checkPassword)
                    {
                        ModelState.AddModelError("Input.Password", "Password is incorrect");
                        await LoadCurrentImage();
                    }
                    else
                    {
                        string? imagePath = null;
                        if (Input.NewImage is { Length: > 0 })
                        {
                            var allowedExtensions = new[] { ".jpg", ".png", ".jpeg", ".webp" };
                            var extension = Path.GetExtension(Input.NewImage.FileName).ToLowerInvariant();

                            if (!allowedExtensions.Contains(extension))
                            {
                                ModelState.AddModelError(string.Empty, "Only image files are allowed");
                                await LoadCurrentImage();
                                return Page();
                            }
                            var fileName = Guid.NewGuid() + Path.GetExtension(Input.NewImage.FileName);
                            var path = Path.Combine("wwwroot/uploads/profiles", fileName);
                            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                            using var stream = new FileStream(path, FileMode.Create);
                            await Input.NewImage.CopyToAsync(stream);
                            imagePath = $"/uploads/profiles/{fileName}";
                        }

                        var oldImage = user.ProfileImage;
                        if (imagePath is not null)
                        {
                            user.ProfileImage = imagePath;
                        }
                        user.FName = Input.FName;
                        user.LName = Input.LName;
                        user.PhoneNumber = Input.PhoneNumber;

                        var result = await _userManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            var claims = await _userManager.GetClaimsAsync(user);
                            var oldFName = claims.FirstOrDefault(claim => claim.Type == "FName");
                            var oldLName = claims.FirstOrDefault(claim => claim.Type == "LName");
                            var oldProfileImage = claims.FirstOrDefault(claim => claim.Type == "Profile");
                            if (oldFName is not null)
                            {
                                await _userManager.ReplaceClaimAsync(user, oldFName, new Claim("FName", user.FName ?? ""));
                            }
                            if (oldLName is not null)
                            {
                                await _userManager.ReplaceClaimAsync(user, oldLName, new Claim("LName", user.LName ?? ""));
                            }
                            if (oldProfileImage is not null)
                            {
                                await _userManager.ReplaceClaimAsync(user, oldProfileImage, new Claim("Profile", user.ProfileImage ?? ""));
                            }

                            if (!string.IsNullOrEmpty(oldImage))
                            {
                                var oldImagePath = Path.Combine(
                                    _environment.WebRootPath,
                                    oldImage.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
                                );

                                if (System.IO.File.Exists(oldImagePath))
                                {
                                    System.IO.File.Delete(oldImagePath);
                                }
                            }

                            await _signinManager.RefreshSignInAsync(user);
                            return RedirectToPage();
                        }

                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return Page();
        }
        private async Task LoadCurrentImage()
        {
            var user = await _userManager.GetUserAsync(User);

            Input.CurrentImage = user!.ProfileImage;
        }
    }
}