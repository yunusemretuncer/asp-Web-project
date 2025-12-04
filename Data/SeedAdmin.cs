namespace AspWebProject.Data
{
    using AspWebProject.Models;
    using Microsoft.AspNetCore.Identity;
    using AspWebProject.Models;

    public static class SeedAdmin
    {
        public static async Task InitializeAsync(UserManager<ApplicationUser> userManager)
        {
            string adminEmail = "ogrencinumarasi@sakarya.edu.tr";
            string adminPassword = "sau";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Admin User"
                };

                var result = await userManager.CreateAsync(user, adminPassword);

                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }

}
