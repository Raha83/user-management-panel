using System.Globalization;
using ManagementPanel;
using ManagementPanel.Services;
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
}).AddEntityFrameworkStores<ManagementContext>().AddDefaultTokenProviders();
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings")
);
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromMinutes(5);
});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
});
builder.Services.AddScoped<EmailSender>();
builder.Services.AddRazorPages();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapRazorPages();

app.Run();
