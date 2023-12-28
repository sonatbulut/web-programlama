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

        
        public async Task<IActionResult> CreateAppointment(int? doctorId)
        {
            
            ViewBag.Doctors = new SelectList(await _context.Doctors.ToListAsync(), "DoctorId","DocNameSurname");
            ViewBag.Patients = new SelectList(await _context.Patients.ToListAsync(), "PatientId","PatientNameSurname");
            DateTime selectedDate = DateTime.Today;
            ViewBag.AvailableTimes = await GetAvailableTimes(selectedDate);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>CreateAppointment(Appointment model,string appointmentTime)
        {
            if (TimeSpan.TryParseExact(appointmentTime.Trim(), "hh\\:mm", CultureInfo.InvariantCulture, out var parsedAppointmentTime))
            {
                // Tarihi ve saati birleştir
                model.AppointmentDate = model.AppointmentDate.Date + parsedAppointmentTime;
            }
            else
            {
                // Hatalı saat formatı durumunda hata ekle ve formu geri göster
                ModelState.AddModelError("AppointmentTime", "Invalid time format.");
                return View(model);
            }
            _context.Appointments.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","Home");
        }


        public async Task<IEnumerable<SelectListItem>> GetAvailableTimes(DateTime appointmentDate)
        {
            var startTime = new TimeSpan(9, 0, 0); // Günün başlangıç saati: 09:00
            var endTime = new TimeSpan(17, 0, 0); // Günün bitiş saati: 17:00
            var timeInterval = 30; // Randevular arası zaman aralığı (dakika)

            // O gün için olan randevuları çek
            var bookedAppointments = await _context.Appointments
                .Where(a => a.AppointmentDate.Date == appointmentDate.Date)
                .Select(a => a.AppointmentDate.TimeOfDay)
                .ToListAsync();

            var availableTimes = new List<SelectListItem>();

            // Başlangıç saatinden, bitiş saatine kadar döngü oluştur
            for (var time = startTime; time < endTime; time = time.Add(TimeSpan.FromMinutes(timeInterval)))
            {
                // Zaten alınmış bir randevu saati değilse listeye ekle
                if (!bookedAppointments.Contains(time))
                {
                    availableTimes.Add(new SelectListItem { Text = time.ToString(@"hh\:mm"), Value = time.ToString(@"hh\:mm") });
                }
            }

            return availableTimes;
        }


        
        [HttpGet]
        public async Task<IActionResult> EditAppointment(int? id)
        {
            ViewBag.Doctors = new SelectList(await _context.Doctors.ToListAsync(), "DoctorId","DocNameSurname");
            ViewBag.Patients = new SelectList(await _context.Patients.ToListAsync(), "PatientId","PatientNameSurname");
            DateTime selectedDate = DateTime.Today;
            ViewBag.AvailableTimes = await GetAvailableTimes(selectedDate);
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