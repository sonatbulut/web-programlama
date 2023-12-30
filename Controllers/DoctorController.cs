using HospitaAppointmentSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitaAppointmentSystem.Controllers
{
    [Authorize(Roles = "admin")]
    public class DoctorController: Controller
    {
        private readonly DataContext _context;
        public DoctorController(DataContext context)
        {   
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Doctors.ToListAsync());
        }

        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        
        public async Task<IActionResult> Create(Doctor model,IFormFile imageFile)
        {
            var extension = Path.GetExtension(imageFile.FileName);
            var randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extension}");
            var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/img",randomFileName);
            if (ModelState.IsValid)
            {
                using (var stream = new FileStream(path,FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                model.Image=randomFileName;
            }
            _context.Doctors.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
            
        }
        [HttpGet]
        public async Task<IActionResult> EditDoc(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }
            var doctor = await _context
                                .Doctors
                                .Include(o=>o.Appointments)
                                .ThenInclude(o=>o.Patient)
                                .FirstOrDefaultAsync(o=>o.DoctorId==id);
            if (doctor==null)
            {
                return NotFound();
            }
            return View(doctor);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDoc(int id, Doctor model)
        {
            if(id!= model.DoctorId){
                return NotFound();
            }

            if(ModelState.IsValid){
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if(!_context.Doctors.Any(o=> o.DoctorId == model.DoctorId)){
                        return NotFound();
                    }
                    else{
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteDoc(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            return View(doctor);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDoc([FromForm]int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}