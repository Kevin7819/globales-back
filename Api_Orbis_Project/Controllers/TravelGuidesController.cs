using Microsoft.AspNetCore.Mvc;
using Api_Orbis_Project.Models;
using Api_Orbis_Project.Services;

[ApiController]
[Route("api/[controller]")]
public class TravelGuidesController : ControllerBase
{
    private readonly ITravelGuideService _guideService;

    public TravelGuidesController(ITravelGuideService guideService)
    {
        _guideService = guideService;
    }

    [HttpGet("quick/{countryCode}")]
    public async Task<ActionResult<QuickGuideResponse>> GetQuickGuide(string countryCode, string lang = "es")
        => Ok(await _guideService.GetQuickGuideAsync(countryCode, lang));

    [HttpGet("safety/{countryCode}")]
    public async Task<ActionResult<SafetyGuide>> GetSafetyGuide(string countryCode, string lang = "es")
        => Ok(await _guideService.GetSafetyGuideAsync(countryCode, lang));

    [HttpGet("health/{countryCode}")]
    public async Task<ActionResult<HealthGuide>> GetHealthGuide(string countryCode, string lang = "es")
        => Ok(await _guideService.GetHealthGuideAsync(countryCode, lang));

    [HttpGet("culture/{countryCode}")]
    public async Task<ActionResult<CultureGuide>> GetCultureGuide(string countryCode, string lang = "es")
        => Ok(await _guideService.GetCultureGuideAsync(countryCode, lang));
}
