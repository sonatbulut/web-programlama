using HospitaAppointmentSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitaAppointmentSystem.Controllers
{
    [Authorize(Roles = "admin")]
    public class PatientController: Controller
    {
        private readonly DataContext _context;
        public PatientController(DataContext context)
        {   
            _context = context;
        }

        public async Task<IActionResult> IndexPatient()
        {
            return View(await _context.Patients.ToListAsync());
        }
        public IActionResult CreatePatient()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> CreatePatient(Patient model)
        {
            _context.Patients.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("IndexPatient");
        }

        [HttpGet]
        public async Task<IActionResult> EditPatient(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }
            var patient = await _context
                                .Patients
                                .Include(o=>o.Appointments)
                                .ThenInclude(o =>o.Doctor)
                                .FirstOrDefaultAsync(o=>o.PatientId==id);
            if (patient==null)
            {
                return NotFound();
            }
            return View(patient);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPatient(int id, Patient model)
        {
            if(id!= model.PatientId){
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
                    if(!_context.Patients.Any(o=> o.PatientId == model.PatientId)){
                        return NotFound();
                    }
                    else{
                        throw;
                    }
                }
                return RedirectToAction("IndexPatient");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeletePatient(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePatient([FromForm]int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return RedirectToAction("IndexPatient");
        }
    }
}