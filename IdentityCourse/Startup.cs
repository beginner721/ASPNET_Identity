using IdentityCourse.CustomValidation;
using IdentityCourse.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityCourse
{
    public class Startup
    {
        public IConfiguration configuration { get; }

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionString"));
            });

            CookieBuilder cookieBuilder = new CookieBuilder();
            cookieBuilder.Name = "MySite";
            cookieBuilder.HttpOnly = false; // burada k�t� niyetli kullan�c�lar� engelliyoruz. Client taraf�nda cookie mize eri�emiyorlar.
                                            // cookieBuilder.Expiration = TimeSpan.FromDays(7); //Bunu ConfigureApplicationCookie yazmal�y�z? Acaba Neden ? ��nk� altta eziliyor......
            cookieBuilder.SameSite = SameSiteMode.Lax;
            //Bir cookie kaydedildikten sonra sadece o site �zerinden bu cookie ye ula�abilirim.
            //Farkl� sitelerde k�t� niyetli scriptler ile cookie tetiklenmesi �nlenir. (Strict Modunda)
            //Lax modunda ise bu g�venlik kapal�d�r. �nemli bilgiler bulunan bankac�l�k gibi bir uygulama de�ilse gerek yok.

            cookieBuilder.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            //�stek sadece https �zerinden gelirse cookie g�nderilir (Always)
            //�stek https ile gelirse https ile cookie g�nderilir, http ile gelirse http ile g�nderilir(SameAsReq.)
            //None olursa hepsi http �zerinden gider. Https protokolu varsa Always olmal�.

            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "������abcdefghijklmnopqrstuvwxyz�����ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; //t�rk�e karakterler eklendi.


                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;

            })
                .AddPasswordValidator<CustomPasswordValidator>()
                .AddUserValidator<CustomUserValidator>()
                .AddErrorDescriber<CustomIdentityErrorDescriber>()
                .AddEntityFrameworkStores<AppIdentityDbContext>();

            //AddIdentity'den sonra Cookie konfigure edilmelidir, ��nk� AddIdentity'de de cookie ayar� mevcut. Override edip ge�ersiz k�labilir.
            services.ConfigureApplicationCookie(options =>
            {


                options.LoginPath = new PathString("/Home/Login");
                //options.LogoutPath = new PathString("Home/");
                options.Cookie = cookieBuilder;
                options.SlidingExpiration = true; //verilen expiration un yar�s�na gelince, expirationu verilen de�er kadar tekrar art�r�r.
                options.ExpireTimeSpan = TimeSpan.FromDays(7);//buraya verilmesinin sebebi cookie configure ederken cookieBuilder eziliyor....
            });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage(); //hata al�nca hata ile alakal� a��klay�c� bilgiler al�yoruz.
            app.UseStatusCodePages(); //i�erik d�nmeyen sayfalarda bilgilendirici yaz�lar g�rmekteyiz.
            app.UseStaticFiles();

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
