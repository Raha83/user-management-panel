using System.Globalization;
using ManagementPanel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ManagementContext>(options =>
options.UseLazyLoadingProxies().UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<User, Role>(options =>
{
    options.User.RequireUniqueEmail = true;

    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;

    options.SignIn.RequireConfirmedEmail = true;
}).AddErrorDescriber<PersianIdentityErrorDescriber>().AddEntityFrameworkStores<ManagementContext>().AddDefaultTokenProviders();
builder.Services.AddRazorPages();
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapRazorPages();

app.MapGet("/", () => "Hello World!");

app.Run();
