using System.ComponentModel.DataAnnotations;

namespace HospitaAppointmentSystem.Data
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; }=null!;
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }=null!;
        public DateTime AppointmentDate { get; set; }
    }
}