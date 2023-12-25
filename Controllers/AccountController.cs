using HospitaAppointmentSystem.Data;
using HospitaAppointmentSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HospitaAppointmentSystem.Controllers
{
    public class AccountController:Controller
    {
        private UserManager<AppUser> _userMnanager;
        private RoleManager<AppRole> _roleManager;
        private SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager,RoleManager<AppRole> roleManager,SignInManager<AppUser> signInManager)
        {
            _userMnanager = userManager;
            _roleManager=roleManager;
            _signInManager = signInManager;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userMnanager.FindByEmailAsync(model.Email);
                if(user!=null)
                {
                    await _signInManager.SignOutAsync();
                    var result = await _signInManager.PasswordSignInAsync(user,model.Password,model.RememberMe,false);

                    if(result.Succeeded)
                    {
                        return RedirectToAction("Index","Home");
                    }
                    else
                    {
                        ModelState.AddModelError("","Wrong password");
                    }
                }
                else
                {
                    ModelState.AddModelError("","Wrong email");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = new AppUser { 
                    UserName = model.UserName,
                    Email = model.Email, 
                    FullName = model.FullName 
                };

                IdentityResult result = await _userMnanager.CreateAsync(user, model.Password);

                if(result.Succeeded)
                {
                    return RedirectToAction("Login","Account");
                }

                foreach (IdentityError err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);                    
                }

            }
            return View(model);
        }
    }
}