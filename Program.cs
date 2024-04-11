using IntexLegoSecure.Models;
using IntexLegoSecure.Data;
using IntexLegoSecure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.Scripting;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
//using IntexLegoSecure.Areas.Identity.Data;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //Initialize Key Vault Client
        //var keyVaultUrl = builder.Configuration["KeyVault:VaultUri"]; // Ensure you have this in your appsettings or as an environment variable
        //var secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());

        //Fetch secrets from Azure Key Vault
        //var googleClientId = secretClient.GetSecret("Google-Client-ID").Value.Value;
        //var googleClientSecret = secretClient.GetSecret("Google-Client-Secret").Value.Value;

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddScoped<I_Repository, EF_Repository>();

        builder.Services.AddRazorPages();

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession();

        builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        builder.Services.AddControllersWithViews();

        ////Configure Google Authentication with Key Vault secrets
        //builder.Services.AddAuthentication().AddGoogle(googleOptions =>
        //{
        //    googleOptions.ClientId = googleClientId;
        //    googleOptions.ClientSecret = googleClientSecret;
        //});

        builder.Services.AddScoped<UserManager<ApplicationUser>>();

        //Google Authentication
        builder.Services.AddAuthentication().AddGoogle(googleOptions =>
        {
            googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
            googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        });


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseSession();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();

        using (var scope = app.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string email = "admin2024@gmail.com";
            string password = "IamAdmin2024!";

            if (await userManager.FindByEmailAsync(email) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = email,
                    Email = email
                };

                await userManager.CreateAsync(admin, password);
                await userManager.AddToRoleAsync(admin, "ADMIN");
            }
        }

        app.MapDefaultControllerRoute();

        app.Run();
    }
}
