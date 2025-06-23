using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wirin.Application;
using Wirin.Domain.Dtos.OCR;
using Wirin.Domain.Providers;
using Wirin.Domain.Services;

namespace Wirin.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
//[Authorize]

public class OcrController : ControllerBase
{
   private ProcessWithLocalOcrUseCase _processWithLocalOcrUseCase;
    private readonly UserService _userService;


    public OcrController(ProcessWithLocalOcrUseCase processWithLocalOcrUseCase, UserService userService)
    {
        _processWithLocalOcrUseCase = processWithLocalOcrUseCase;
        _userService = userService;
    }

    [HttpPost("{engine}")]
    [ProducesResponseType(typeof(OcrResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ProcessOcr(string engine, [FromQuery] int id)
    {
        var trasabilityUserId = _userService.GetUserTrasabilityId(User); // <- ESTA LNEA
        try
        {
            var result = await _processWithLocalOcrUseCase.__Invoke(engine, id, trasabilityUserId);

            if (result == null)
            {
                return NotFound(new { error = "No se encontraron resultados para el OCR." });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }


}
