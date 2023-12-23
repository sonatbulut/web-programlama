using System.ComponentModel.DataAnnotations;

namespace HospitaAppointmentSystem.ViewModels
{
    public class EditViewModel
    {
        public string? Id { get; set; }
        public string? FullName { get; set; } 
        
        [EmailAddress]
        public string? Email { get; set; } 

        
        [DataType(DataType.Password)]
        public string? Password { get; set; } 

        
        [DataType(DataType.Password)]
        [Compare(nameof(Password),ErrorMessage = "Pasword does not match")]
        public string? ConfirmPassword { get; set; } 

    }
}