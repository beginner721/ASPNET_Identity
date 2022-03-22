using IdentityCourse.Models;
using IdentityCourse.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult; //Çakışma oluyor o sebeple burası usingden eklendi. AspNetCore.Mvc de de Identity var.

namespace IdentityCourse.Controllers
{
    public class HomeController : Controller
    {
        public UserManager<AppUser> _userManager { get; }
        public SignInManager<AppUser> _signInManager { get; }
        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            TempData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel userLogin)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = await _userManager.FindByEmailAsync(userLogin.Email);
                if (appUser != null)
                {
                    if (await _userManager.IsLockedOutAsync(appUser))
                    {
                        ModelState.AddModelError("", "Hesabınız güvenlik için kilitlenmiştir. Daha sonra tekrar deneyin.");
                        return View(userLogin);
                    }
                    await _signInManager.SignOutAsync(); //Önce çıkış yapıyoruz, farklı bir cookie varsa silinsin vs.
                    SignInResult result = await _signInManager.PasswordSignInAsync(appUser, userLogin.Password, userLogin.RememberMe, false);
                    if (result.Succeeded)
                    {
                        await _userManager.ResetAccessFailedCountAsync(appUser);//Giriş yapınca, başarısız giriş denemelerini sıfırlar.
                        if (TempData["ReturnUrl"]!=null)
                        {
                            return Redirect(TempData["ReturnUrl"].ToString());
                        }
                        return RedirectToAction("Index", "Member");//Buradan farklı bi sayfadan geldiyse üst kısım çalışacaktır.
                    }
                    else
                    {
                        await _userManager.AccessFailedAsync(appUser);
                        int failedLoginCount = await _userManager.GetAccessFailedCountAsync(appUser);
                        if (failedLoginCount==3)
                        {
                            await _userManager.SetLockoutEndDateAsync(appUser, DateTime.Now.AddMinutes(30));
                            ModelState.AddModelError("", "Hesabınıza 3 başarısız giriş yapılmıştır. 30dk sonra tekrar giriş yapmayı deneyebilirsiniz");
                            return View(userLogin);
                        }
                        ModelState.AddModelError("", $"{failedLoginCount} kere başarısız giriş yapıldı. ");
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Böyle bir E-posta adresi mevcut değil, üye olmak ister misiniz?");
                }
                
            }
            return View(userLogin);
        }

        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser();
                user.UserName = userViewModel.UserName;
                user.Email = userViewModel.Email;
                user.PhoneNumber = userViewModel.PhoneNumber;

                IdentityResult result = await _userManager.CreateAsync(user, userViewModel.Password);
                //result üzerinden hataları yakalayabiliriz. başarılı-başarısız durumları sorgulanabilir.

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                        //ilk parametre boş bırakılır ise index kısmındaki validation summary(özet) tarafına gidecektir bütün hatalar. 
                    }
                }
            }


            return View(userViewModel);

        }
    }
}
