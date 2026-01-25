namespace AspWebProject.Models
{
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser
    {

        public string? FullName { get; set; }
        public int? Height { get; set; }
        public int? Weight { get; set; }
        public string? BodyType { get; set; }
    }
}
