using erpv01.Data;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

namespace erpv01
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthentication("TrinexCookieAuth")
    .AddCookie("TrinexCookieAuth", options =>
    {
        options.Cookie.Name = "TrinexERP.Auth";
        options.LoginPath = "/Account/Login"; // Giriþ yapýlmamýþsa buraya atar
        options.AccessDeniedPath = "/Account/AccessDenied"; // Yetkisiz giriþ
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Oturum süresi
    });


            QuestPDF.Settings.License = LicenseType.Community;

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                // 2. appsettings.json'daki "DefaultConnection" stringini kullan
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
