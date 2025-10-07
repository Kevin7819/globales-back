using Microsoft.AspNetCore.Mvc;

namespace Api_Orbis_Project.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AiController : ControllerBase
    {
        private readonly HuggingFaceService _huggingFace;

        public AiController(HuggingFaceService huggingFace)
        {
            _huggingFace = huggingFace;
        }

        [HttpGet("ask")]
        public async Task<IActionResult> Ask([FromQuery] string question, [FromQuery] string lang = "es")
        {
            if (string.IsNullOrWhiteSpace(question))
                return BadRequest(new { error = "Question cannot be empty." });

            try
            {
                var answer = await _huggingFace.AskAsync(question, lang);
                return Ok(new { answer });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new { error = "Error calling Hugging Face API", details = ex.Message });
            }
        }
    }
}
