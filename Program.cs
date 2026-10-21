using ManagementPanel;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ManagementContext>(options =>
options.UseLazyLoadingProxies().UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
