using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitaAppointmentSystem.Data
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public string? PatientSurname { get; set; }
        public string PatientNameSurname 
        { 
            get
            {
                return this.PatientName + " " + this.PatientSurname;
            } 
        }
        public string? PatientEmail { get; set; }
        public string? PatientPhone { get; set; }
        public ICollection<Appointment> Appointments { get; set; }= new List<Appointment>();
    }
}