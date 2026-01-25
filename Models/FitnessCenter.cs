namespace AspWebProject.Models
{
    using System.ComponentModel.DataAnnotations;

    public class FitnessCenter
    {
        public int Id { get; set; }


        public required string Name { get; set; }


        // Çalışma saatleri artık TimeSpan olarak tutuluyor
        [Required]
        [Display(Name = "Açılış Saati")]
        public TimeSpan OpenTime { get; set; }

        [Required]
        [Display(Name = "Kapanış Saati")]
        public TimeSpan CloseTime { get; set; }
    }

}
