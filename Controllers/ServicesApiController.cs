using AspWebProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ServicesApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ServicesApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/services/filter?fitnessCenterId=1
    [HttpGet("filter")]
    public async Task<IActionResult> Filter(int fitnessCenterId = 0)
    {
        var query = _context.Services
            .Include(s => s.FitnessCenter)
            .AsQueryable();

        if (fitnessCenterId > 0)
        {
            query = query.Where(s => s.FitnessCenterId == fitnessCenterId);
        }

        var result = await query
            .Select(s => new
            {
                s.Id,
                s.Name,
                s.Duration,
                s.Price,
                FitnessCenter = s.FitnessCenter != null ? s.FitnessCenter.Name : string.Empty
            })
            .ToListAsync();

        return Ok(result);
    }
}
