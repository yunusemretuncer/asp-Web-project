using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

public class AiFitnessController : Controller
{
    private readonly GroqService _groq;

    public AiFitnessController(GroqService groq)
    {
        _groq = groq;
    }

    public IActionResult Index() => View();

    [HttpPost]
    public async Task<IActionResult> Index(FitnessRequestModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        string prompt =
            $"Boy: {model.Height} cm, Kilo: {model.Weight} kg, Vücut tipi: {model.BodyType}. " +
            $"Bana detaylı bir kişisel fitness programı oluştur.";

        try
        {
            model.Answer = await _groq.GeneratePlan(prompt);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(string.Empty, "Unable to contact the Groq service. Please try again later.");
            return View(model);
        }

        return View(model);
    }
}

public class FitnessRequestModel
{
    [Range(50, 300)]
    public int Height { get; set; }

    [Range(20, 300)]
    public int Weight { get; set; }

    [Required]
    public required string BodyType { get; set; }

    public string? Answer { get; set; }
}
