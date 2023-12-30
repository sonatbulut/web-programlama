using HospitaAppointmentSystem.Data;
using HospitaAppointmentSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitaAppointmentSystem.Controllers
{
    [Authorize(Roles ="admin")]
    public class UsersController:Controller
    {

        private UserManager<AppUser> _userMnanager;
        private RoleManager<AppRole> _roleManager;

        public UsersController(UserManager<AppUser> userManager,RoleManager<AppRole> roleManager)
        {
            _userMnanager = userManager;
            _roleManager=roleManager;
        }
      

        public IActionResult Index()
        {
            return View(_userMnanager.Users);
        }

        public IActionResult Create()
        {
            return View();
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
                ViewBag.Roles=await _roleManager.Roles.Select(i=>i.Name).ToListAsync();
                return View(new EditViewModel{
                    Id=user.Id,
                    FullName=user.FullName,
                    Email=user.Email,
                    SelectedRoles= await _userMnanager.GetRolesAsync(user)
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
                        await _userMnanager.RemoveFromRolesAsync(user,await _userMnanager.GetRolesAsync(user));
                        if(model.SelectedRoles!=null)
                        {
                            await _userMnanager.AddToRolesAsync(user,model.SelectedRoles);
                        }
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