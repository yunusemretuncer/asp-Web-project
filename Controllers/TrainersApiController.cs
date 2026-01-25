using AspWebProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class TrainersApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TrainersApiController(ApplicationDbContext context)
    {
        _context = context;
    }



    [HttpGet("filter")]
    public async Task<IActionResult> Filter(int? fitnessCenterId, DayOfWeek? day, int? serviceId)
    {
        var query = _context.Trainers
            .Include(t => t.FitnessCenter)
            .Include(t => t.TrainerServices!)
                .ThenInclude(ts => ts.Service)
            .Include(t => t.Availabilities)
            .AsQueryable();

        // Fitness center filtrele
        if (fitnessCenterId.HasValue)
            query = query.Where(t => t.FitnessCenterId == fitnessCenterId.Value);

        // Gün filtrele
        if (day.HasValue)
            query = query.Where(t => t.Availabilities != null && t.Availabilities.Any(a => a.Day == day.Value));

        // Hizmete göre filtrele
        if (serviceId.HasValue)
            query = query.Where(t => t.TrainerServices != null && t.TrainerServices.Any(ts => ts.ServiceId == serviceId.Value));

        var result = await query
            .Select(t => new
            {
                t.Id,
                t.FullName,
                t.Expertise,
                FitnessCenter = t.FitnessCenter != null ? t.FitnessCenter.Name : string.Empty
            })
            .ToListAsync();

        return Ok(result);
    }

}
