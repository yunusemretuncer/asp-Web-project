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
    string? status,
    string? userId,
    string mode = "future"
)
    {
        var query = _context.Appointments
            .Include(a => a.Service)
            .Include(a => a.Trainer)
            .Include(a => a.FitnessCenter)
            .Include(a => a.User)
            .AsQueryable();

        // 🔥 GELECEK / GEÇMİŞ FİLTRESİ (EN ÖNEMLİ KISIM)
        DateTime now = DateTime.Now;

        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        bool isAdmin = User.IsInRole("Admin");

        // 🔥 Admin değilse → sadece kendi randevularını görür
        if (!isAdmin)
        {
            query = query.Where(a => a.UserId == currentUserId);
        }

        if (mode == "future")
            query = query.Where(a => a.Date >= now);

        if (mode == "past")
            query = query.Where(a => a.Date < now);

        // 🔥 Diğer filtreler
        if (startDate.HasValue)
            query = query.Where(a => a.Date.Date >= startDate.Value.Date);

        if (serviceId.HasValue && serviceId > 0)
            query = query.Where(a => a.ServiceId == serviceId);

        if (trainerId.HasValue && trainerId > 0)
            query = query.Where(a => a.TrainerId == trainerId);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(a => a.Status.ToString() == status);

        if (!string.IsNullOrEmpty(userId) && userId != "all")
            query = query.Where(a => a.UserId == userId);

        var result = await query
            .OrderBy(a => a.Date)
            .Select(a => new
            {
                id = a.Id,
                date = a.Date.ToString("yyyy-MM-dd HH:mm"),
                service = a.Service != null ? a.Service.Name : string.Empty,
                trainer = a.Trainer != null ? a.Trainer.FullName : string.Empty,
                center = a.FitnessCenter != null ? a.FitnessCenter.Name : string.Empty,
                user = a.User != null ? a.User.UserName : string.Empty,
                status = a.Status.ToString()
            })
            .ToListAsync();

        return Ok(result);
    }


}
