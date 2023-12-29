using System.Globalization;
using HospitaAppointmentSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HospitaAppointmentSystem.Controllers
{
    public class AppointmentController:Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly DataContext _context;

        public AppointmentController(UserManager<AppUser> userManager,DataContext context)
        {
            _context = context;
            _userManager=userManager;
        }

        public async Task<IActionResult> IndexAppointment()
        {
            var appointments = await _context.Appointments
            .Include(x => x.Doctor)
            .Include(x => x.Patient)
            .ToListAsync();
            return View(appointments);
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAppointmentTimesJson(DateTime selectedDate,int doctorId)
        {
            var times = await GetAppointmentTimes(selectedDate,doctorId);
            return Json(new SelectList(times, "Value", "Text"));
        }

        private async Task<List<SelectListItem>> GetAppointmentTimes(DateTime appointmentDate,int doctorId)
        {

            var selectedTimes = await _context.Appointments
                .Where(r => r.AppointmentDate.Date == appointmentDate.Date && r.DoctorId==doctorId)
                .Select(r => r.AppointmentTime)
                .ToListAsync();

            var times = new List<SelectListItem>();
            var begin = new TimeSpan(9, 0, 0);
            var end = new TimeSpan(17, 0, 0);
            var timeInterval = TimeSpan.FromMinutes(30);

            for (var time = begin; time < end; time += timeInterval)
            {
                if (!selectedTimes.Contains(time))
                {
                    times.Add(new SelectListItem
                    {
                        Value = time.ToString(),
                        Text = time.ToString(@"hh\:mm")
                    });
                }
            }

            return times;
        }
        [HttpGet]
        public async Task<IActionResult> CreateAppointment(int? doctorId)
        {
            
            ViewBag.Doctors = new SelectList(await _context.Doctors.ToListAsync(), "DoctorId","DocNameSurname");
            ViewBag.Patients = new SelectList(await _context.Patients.ToListAsync(), "PatientId","PatientNameSurname");
            // DateTime selectedDate = DateTime.Today;
            // ViewBag.AvailableTimes = await GetAvailableTimes(selectedDate);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>CreateAppointment(Appointment model)
        {
            _context.Appointments.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","Home");
        }
        [HttpGet]
        public async Task<IActionResult> EditAppointment(int? id)
        {
            ViewBag.Doctors = new SelectList(await _context.Doctors.ToListAsync(), "DoctorId","DocNameSurname");
            ViewBag.Patients = new SelectList(await _context.Patients.ToListAsync(), "PatientId","PatientNameSurname");
            // DateTime selectedDate = DateTime.Today;
            // ViewBag.AvailableTimes = await GetAvailableTimes(selectedDate);
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