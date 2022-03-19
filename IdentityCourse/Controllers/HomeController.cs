using IdentityCourse.Models;
using IdentityCourse.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IdentityCourse.Controllers
{
    public class HomeController : Controller
    {
        public UserManager<AppUser> _userManager{ get; }

        public HomeController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
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
                AppUser user= new AppUser();
                user.UserName = userViewModel.UserName;
                user.Email = userViewModel.Email;
                user.PhoneNumber = userViewModel.PhoneNumber;

                IdentityResult result = await _userManager.CreateAsync(user,userViewModel.Password);
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
                        //ilk parametre boş bırakılır ise index kısmındaki validation summary tarafına gidecektir bütün hatalar.
                    }
                }
            }


            return View(userViewModel);

        }
    }
}
