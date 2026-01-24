using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace AspWebProject.Models
{
    public class Trainer
    {
        public int Id { get; set; }

        
        public  string? FullName { get; set; }
        public string? Expertise { get; set; }
        public int FitnessCenterId { get; set; }
        public FitnessCenter? FitnessCenter { get; set; }

        public ICollection<TrainerService>? TrainerServices { get; set; } = new List<TrainerService>();

        public ICollection<Appointment>? Appointments { get; set; } = new List<Appointment>();
        public ICollection<TrainerAvailability>? Availabilities { get; set; } = new List<TrainerAvailability>();
    }

}
