using System.Drawing;
using HospitaAppointmentSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HospitaAppointmentSystem.Controllers
{
    public class AppointmentController:Controller
    {
        private readonly DataContext _context;

        public AppointmentController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> IndexAppointment()
        {
            var appointments = await _context.Appointments
            .Include(x => x.Doctor)
            .Include(x => x.Patient)
            .ToListAsync();
            return View(appointments);
        }

        
        public async Task<IActionResult> CreateAppointment()
        {
            ViewBag.Doctors = new SelectList(await _context.Doctors.ToListAsync(), "DoctorId","DocNameSurname");
            ViewBag.Patients = new SelectList(await _context.Patients.ToListAsync(), "PatientId","PatientNameSurname");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>CreateAppointment(Appointment model)
        {
            _context.Appointments.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("IndexAppointment");
        }


        [HttpGet]
        public async Task<IActionResult> EditAppointment(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment==null)
            {
                return NotFound();
            }
            return View(appointment);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAppointment(int id, Appointment model)
        {
            if(id!= model.AppointmentId){
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
                    if(!_context.Appointments.Any(o=> o.AppointmentId == model.AppointmentId)){
                        return NotFound();
                    }
                    else{
                        throw;
                    }
                }
                return RedirectToAction("IndexAppointment");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAppointment(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            return View(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAppointment([FromForm]int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction("IndexAppointment");
        }

    }
}