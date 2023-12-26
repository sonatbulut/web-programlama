using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HospitaAppointmentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using HospitaAppointmentSystem.Data;

namespace HospitaAppointmentSystem.Controllers;
//[Authorize]

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly DataContext _context;
    private readonly IDoctorRepository _doctorRepository;

    public HomeController(ILogger<HomeController> logger,DataContext context,IDoctorRepository doctorRepository)
    {
        _logger = logger;
        _doctorRepository=doctorRepository;
        _context=context;

    }
    public IActionResult Index()
    {
        var doctors = _doctorRepository.GetAllDoctors();
        return View(doctors);
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
