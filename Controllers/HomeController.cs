using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HospitaAppointmentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using HospitaAppointmentSystem.Data;
using Microsoft.AspNetCore.Localization;
using HospitaAppointmentSystem.Services;

namespace HospitaAppointmentSystem.Controllers;
//[Authorize]

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly DataContext _context;
    private readonly IDoctorRepository _doctorRepository;
    private LanguageService _localization;

    public HomeController(ILogger<HomeController> logger,DataContext context,IDoctorRepository doctorRepository,LanguageService localization)
    {
        _logger = logger;
        _doctorRepository=doctorRepository;
        _context=context;
        _localization=localization;

    }
    public IActionResult Index()
    {
        ViewBag.Doctors= _localization.Getkey("Doctors").Value;
        var doctors = _doctorRepository.GetAllDoctors();
        var currentCulture = Thread.CurrentThread.CurrentCulture.Name;
        return View(doctors);
    }
    public IActionResult ChangeLanguage(string culture)
    {
        Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),new CookieOptions()
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1)
        });
        return Redirect(Request.Headers["Referer"].ToString());
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
