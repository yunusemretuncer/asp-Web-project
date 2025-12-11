using AspWebProject.Data;
using AspWebProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class AppointmentsApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AppointmentsApiController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: api/appointments/filter
    [HttpGet("filter")]
    public async Task<IActionResult> Filter(
    DateTime? startDate,
    int? serviceId,
    int? trainerId,
    AppointmentStatus? status,
    string? userId)
    {
        var query = _context.Appointments
            .Include(a => a.Service)
            .Include(a => a.Trainer)
            .Include(a => a.FitnessCenter)
            .Include(a => a.User)
            .AsQueryable();

        // Eğer admin değilse → sadece kendi randevuları
        if (!User.IsInRole("Admin"))
        {
            query = query.Where(a => a.UserId == _userManager.GetUserId(User));
        }
        else
        {
            if (!string.IsNullOrEmpty(userId) && userId != "all")
                query = query.Where(a => a.UserId == userId);
        }

        if (startDate != null)
            query = query.Where(a => a.Date.Date >= startDate.Value.Date);

        if (serviceId != null && serviceId > 0)
            query = query.Where(a => a.ServiceId == serviceId);

        if (trainerId != null && trainerId > 0)
            query = query.Where(a => a.TrainerId == trainerId);

        if (status != null)
            query = query.Where(a => a.Status == status);

        var result = await query
            .OrderBy(a => a.Date)
            .Select(a => new {
                a.Id,
                date = a.Date.ToString("yyyy-MM-dd HH:mm"),
                service = a.Service.Name,
                trainer = a.Trainer.FullName,
                center = a.FitnessCenter.Name,
                status = a.Status.ToString(),
                user = a.User.UserName // ADMIN için
            })
            .ToListAsync();

        return Ok(result);
    }
}
