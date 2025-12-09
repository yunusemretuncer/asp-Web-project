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

    // GET: /api/trainers
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var trainers = await _context.Trainers
            .Select(t => new {
                t.Id,
                t.FullName,
                t.Expertise
            })
            .ToListAsync();

        return Ok(trainers);
    }

    // GET: /api/trainers/available?date=2025-12-03&time=11:00
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable(DateTime date, TimeSpan time)
    {
        var trainers = await _context.Trainers
            .Where(t => t.Availabilities.Any(a =>
                a.Day == date.DayOfWeek &&
                a.StartTime <= time &&
                a.EndTime >= time
            ))
            .ToListAsync();

        return Ok(trainers);
    }
}
