using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManagementPanel.Pages.Account
{
    public class ConfirmationModel : PageModel
    {
        public string? Message { get; set; }
        public void OnGetAsync(string message)
        {
            Message = message;
        }
    }
}