namespace AspWebProject.Models.ViewModels
{
    public class TrainerEditViewModel
    {
        public Trainer Trainer { get; set; }

        public List<Service>? AllServices { get; set; }

        public List<int>? SelectedServiceIds { get; set; }

        public List<AvailabilityInput>? Availabilities { get; set; } = new();
    }
    public class AvailabilityInput
    {
        public DayOfWeek Day { get; set; }
        public bool Enabled { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
