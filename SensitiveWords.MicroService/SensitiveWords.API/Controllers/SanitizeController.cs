using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Flash.SensitiveWords.Application.DTOs;
using Flash.SensitiveWords.Application.Handlers;
using Flash.SensitiveWords.Application.Queries;

namespace Flash.SensitiveWords.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SanitizeController : ControllerBase
    {
        private readonly SanitizeMessageHandler _sanitizeHandler;

        public SanitizeController(SanitizeMessageHandler sanitizeHandler)
        {
            _sanitizeHandler = sanitizeHandler ?? throw new ArgumentNullException(nameof(sanitizeHandler));
        }

        [HttpPost]
        [ProducesResponseType(typeof(SanitizeResponse), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Sanitize([FromBody] SanitizeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var query = new SanitizeMessageQuery(request.Message);
                var result = await _sanitizeHandler.HandleAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
