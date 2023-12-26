namespace HospitaAppointmentSystem.Data
{
    public interface IDoctorRepository
    {
        IEnumerable<Doctor> GetAllDoctors();
    }
}