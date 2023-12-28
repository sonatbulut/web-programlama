function getAvailableTimesForDate(date) {
    // Sunucuya uygun saatleri getirme isteği yap
    $.ajax({
        url: '/Appointment/GetAvailableTimes', // Bu URL sunucuda bir action'a karşılık gelmeli
        type: 'GET',
        data: { appointmentDate: date },
        success: function (times) {
            // Saat seçim listesini güncelle
            var timeSelect = $('#AppointmentTime');
            timeSelect.empty();
            $.each(times, function (i, time) {
                timeSelect.append($('<option></option>').val(time).text(time));
            });
        }
    });
}

// Tarih değiştiğinde fonksiyonu çağır
$('#AppointmentDate').change(function () {
    var selectedDate = $(this).val();
    getAvailableTimesForDate(selectedDate);
});