using System.ComponentModel.DataAnnotations;

namespace HospitaAppointmentSystem.Data
{
    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }
        public string? DocName { get; set; }
        public string? DocSurname { get; set; }
        public string DocNameSurname 
        { 
            get
            {
                return this.DocName + " " + this.DocSurname;
            }
        }
        public string? DocEmail { get; set; }
        public string? DocSpeacialty { get; set; }
        public string? Image { get; set; }
        public ICollection<Appointment> Appointments { get; set; }= new List<Appointment>();

    }
}