using AxiomAdmin.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(connectionString));

var connectionAdminStrings = builder.Configuration.GetConnectionString("AdminDefaultConnection") ?? throw new InvalidOperationException("Connection string 'AdminDefaultConnection' not found.");
builder.Services.AddDbContext<AdminDbContext>(options =>
    options.UseMySQL(connectionAdminStrings));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AdminDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
// ... (existing code)

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Super-Administration", "Validate-Manager", "Administrator", "Blogger" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    await CreateUserIfNotExists(userManager, "hublinksoft", "superadmin@hublinksoft.com", "SuperAdmin@hublink23", "Super-Administration").ConfigureAwait(false);
}

async Task CreateUserIfNotExists(UserManager<IdentityUser> userManager, string username, string email, string password, string role)
{
    if (await userManager.FindByEmailAsync(email).ConfigureAwait(false) == null)
    {
        var user = new IdentityUser
        {
            UserName = username,
            Email = email,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(user, password).ConfigureAwait(false);
        await userManager.AddToRoleAsync(user, role).ConfigureAwait(false);
    }
}

// ... (remaining code)

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();