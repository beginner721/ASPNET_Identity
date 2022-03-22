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
            cookieBuilder.HttpOnly = false; // burada kötü niyetli kullanýcýlarý engelliyoruz. Client tarafýnda cookie mize eriþemiyorlar.
                                            // cookieBuilder.Expiration = TimeSpan.FromDays(7); //Bunu ConfigureApplicationCookie yazmalýyýz? Acaba Neden ? çünkü altta eziliyor......
            cookieBuilder.SameSite = SameSiteMode.Lax;
            //Bir cookie kaydedildikten sonra sadece o site üzerinden bu cookie ye ulaþabilirim.
            //Farklý sitelerde kötü niyetli scriptler ile cookie tetiklenmesi önlenir. (Strict Modunda)
            //Lax modunda ise bu güvenlik kapalýdýr. Önemli bilgiler bulunan bankacýlýk gibi bir uygulama deðilse gerek yok.

            cookieBuilder.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            //Ýstek sadece https üzerinden gelirse cookie gönderilir (Always)
            //Ýstek https ile gelirse https ile cookie gönderilir, http ile gelirse http ile gönderilir(SameAsReq.)
            //None olursa hepsi http üzerinden gider. Https protokolu varsa Always olmalý.

            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "çðöþýüabcdefghijklmnopqrstuvwxyzÇÐÖÞÜABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; //türkçe karakterler eklendi.


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

            //AddIdentity'den sonra Cookie konfigure edilmelidir, çünkü AddIdentity'de de cookie ayarý mevcut. Override edip geçersiz kýlabilir.
            services.ConfigureApplicationCookie(options =>
            {


                options.LoginPath = new PathString("/Home/Login");
                //options.LogoutPath = new PathString("Home/");
                options.Cookie = cookieBuilder;
                options.SlidingExpiration = true; //verilen expiration un yarýsýna gelince, expirationu verilen deðer kadar tekrar artýrýr.
                options.ExpireTimeSpan = TimeSpan.FromDays(7);//buraya verilmesinin sebebi cookie configure ederken cookieBuilder eziliyor....
            });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage(); //hata alýnca hata ile alakalý açýklayýcý bilgiler alýyoruz.
            app.UseStatusCodePages(); //içerik dönmeyen sayfalarda bilgilendirici yazýlar görmekteyiz.
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
