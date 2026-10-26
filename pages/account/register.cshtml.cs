using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client.NativeInterop;

namespace ManagementPanel.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        public RegisterModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public RegisterInput Input { get; set; } = new();
        public class RegisterInput
        {
            [Display(Name = "نام")]
            public string? FName { get; set; }

            [Display(Name = "نام خانوادگی")]
            public string? LName { get; set; }

            [Required(ErrorMessage = "لطفا ایمیل خود را وارد کنید.")]
            [Display(Name = "ایمیل")]
            [EmailAddress(ErrorMessage = "فرمت ایمیل وارد شده صحیح نیست.")]
            public string Email { get; set; } = string.Empty;

            [Display(Name = "شماره تماس")]
            [DataType(DataType.PhoneNumber)]
            [Phone(ErrorMessage = "فرمت شماره تماس وارد شده صحیح نیست.")]
            public string? PhoneNumber { get; set; }

            [Required(ErrorMessage = "لطفا رمز عبور خود را وارد کنید.")]
            [Display(Name = "رمز عبور")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "لطفا رمز عبور خود را تکرار کنید.")]
            [Display(Name = "تکرار رمز عبور")]
            [DataType(DataType.Password)]
            [Compare(nameof(Password), ErrorMessage = "رمز عبور با تکرار آن مطابقت ندارد.")]
            public string ConfirmPassword { get; set; } = string.Empty;

            [Display(Name = "تصویر پروفایل")]
            [DataType(DataType.ImageUrl, ErrorMessage = "فرمت فایل وارد شده صحیح نیست.")]
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
            var user = new User
            {
                fname = Input.FName,
                lname = Input.LName,
                UserName = Input.Email,
                Email = Input.Email,
                PhoneNumber = Input.PhoneNumber,
                PasswordHash = Input.Password,
                profileImage = imagePath,
                isActive = true
            };
            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                return RedirectToPage("/account/register");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }
    }
}