using Microsoft.AspNetCore.Identity;

namespace HospitaAppointmentSystem.Data
{
    public class AppUser:IdentityUser
    {
        public string? FullName { get; set; } 
    }
}