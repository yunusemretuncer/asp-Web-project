using System.ComponentModel.DataAnnotations.Schema;

namespace AspWebProject.Models
{
    public class Appointment
    {
        public int Id { get; set; }


        public required string UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public int FitnessCenterId { get; set; }
        public FitnessCenter? FitnessCenter { get; set; }

        public int TrainerId { get; set; }
        public Trainer? Trainer { get; set; }

        public int ServiceId { get; set; }
        public Service? Service { get; set; }

        public DateTime Date { get; set; }
        [NotMapped]
        public string HourString { get; set; } = string.Empty;

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    }

}
