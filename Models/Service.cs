namespace AspWebProject.Models
{
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Service
    {
        public int Id { get; set; }


        public required string Name { get; set; }

        public int Duration { get; set; } // dakika
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.1, 9999)]
        public decimal Price { get; set; }

        public int FitnessCenterId { get; set; }
        public FitnessCenter? FitnessCenter { get; set; }
        [ValidateNever]
        public ICollection<TrainerService> TrainerServices { get; set; } = new List<TrainerService>();

    }

}
