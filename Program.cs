using IntexLegoSecure.Models;
using IntexLegoSecure.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.Scripting;
//using IntexLegoSecure.Areas.Identity.Data;

public class Program
{


    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(365); 
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseHsts();
    }


    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;
        var configuration = builder.Configuration;



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


        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        builder.Services.AddControllersWithViews();

        //Google Authentication
        services.AddAuthentication().AddGoogle(googleOptions =>
        {
            googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
            googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
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

        //app.Use(async (context, next) =>
        //{
        //    context.Response.Headers.Add("X-Context-Type-Options", "nosniff");
        //    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        //    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
        //    context.Response.Headers.Add("Content-Security-Policy", "base-uri 'self' frame-src www.google.com; default-src 'self'; script-src 'self' www.google.com; connect-src 'self' google-anaytics.com; img-src data: 'self' www.gstatic.com; style-src 'self' fonts.googleapis.com;");

        //    context.Response.Headers.Remove("X-Powered-By");
        //    context.Response.Headers.Remove("Server");

        //    await next();
        //});


        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();

        using (var scope = app.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string email = "admin2024@gmail.com"; 
            string password = "IamAdmin2024!";

            if (await userManager.FindByEmailAsync(email) == null)
            {
                var admin = new IdentityUser
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



