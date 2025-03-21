using FUBusiness.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Repositories.RepositoriesImpl;
using RestaurantDataAccess;

namespace RestaurantReservationManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<ReservationDAO>();
            builder.Services.AddScoped<UserDAO>();
            builder.Services.AddScoped<TableDAO>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ITableRepository, TableRepository>();
            builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
            builder.Services.AddLogging(logging => logging.AddConsole());
            builder.Services.AddDbContext<RestaurantReservationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );
            builder
                .Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme =
                        CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme =
                        CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.LoginPath = "/Auth/Login";
                    options.LogoutPath = "/Auth/Logout";
                    options.AccessDeniedPath = "/Home/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    options.SlidingExpiration = true;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                });
            ;
            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            app.Run();
        }
    }
}
