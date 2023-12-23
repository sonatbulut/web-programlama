using HospitaAppointmentSystem.Data;
using HospitaAppointmentSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HospitaAppointmentSystem.Controllers
{
    public class UsersController:Controller
    {

        public UsersController(UserManager<AppUser> userManager)
        {
            _userMnanager = userManager;
        }
        private UserManager<AppUser> _userMnanager;

        public IActionResult Index()
        {
            return View(_userMnanager.Users);
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
                var user = new AppUser{UserName=model.Email, Email=model.Email,FullName=model.FullName};
                IdentityResult result= await _userMnanager.CreateAsync(user,model.Password);

                if(result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach(IdentityError err in result.Errors)
                {
                    ModelState.AddModelError("",err.Description);
                }

            }
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if(id==null)
            {
                return RedirectToAction("Index");
            }
            var user = await _userMnanager.FindByIdAsync(id);
            if(user!=null)
            {
                return View(new EditViewModel{
                    Id=user.Id,
                    FullName=user.FullName,
                    Email=user.Email
                });
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id,EditViewModel model)
        {
            if(id!= model.Id)
            {
                return RedirectToAction("Index");
            }

            if(ModelState.IsValid)
            {
                var user=await _userMnanager.FindByIdAsync(model.Id);
                if(user!=null)
                {
                    user.Email=model.Email;
                    user.FullName=model.FullName;

                    var result = await _userMnanager.UpdateAsync(user);

                    if(result .Succeeded && !string.IsNullOrEmpty(model.Password))
                    {
                        await _userMnanager.RemovePasswordAsync(user);
                        await _userMnanager.AddPasswordAsync(user,model.Password);
                    }
                    if(result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    foreach(IdentityError err in result.Errors)
                    {
                        ModelState.AddModelError("",err.Description);
                    }
                }
            }

            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userMnanager.FindByIdAsync(id);

            if (user!=null)
            {
                await _userMnanager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }
    }
}