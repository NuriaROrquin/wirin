using Microsoft.AspNetCore.Mvc;
using Wirin.Api.Dtos.Requests;
using Wirin.Api.Dtos.Response;
using Wirin.Domain.Models;
using Wirin.Domain.Services;
using System.Threading.Tasks;
using System.Linq;

namespace Wirin.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CareerController : ControllerBase
{
    private readonly CareerService _careerService;

    public CareerController(CareerService careerService)
    {
        _careerService = careerService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CareerResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCareers()
    {
        var careers = await _careerService.GetAllCareersAsync();
        return Ok(CareerResponse.ListFromDomain(careers));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CareerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCareerById(int id)
    {
        var career = await _careerService.GetCareerByIdAsync(id);
        if (career == null)
        {
            return NotFound();
        }
        return Ok(CareerResponse.FromDomain(career));
    }

    [HttpPost]
    [ProducesResponseType(typeof(CareerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCareer([FromBody] CreateCareerRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var career = new Career
        {
            Name = request.Name,
            CodDepartamento = request.CodDepartamento
        };

        await _careerService.AddCareerAsync(career);
        return CreatedAtAction(nameof(GetCareerById), new { id = career.Id }, CareerResponse.FromDomain(career));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCareer(int id, [FromBody] UpdateCareerRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingCareer = await _careerService.GetCareerByIdAsync(id);
        if (existingCareer == null)
        {
            return NotFound();
        }

        existingCareer.Name = request.Name;
        existingCareer.CodDepartamento = request.CodDepartamento;

        await _careerService.UpdateCareerAsync(existingCareer);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCareer(int id)
    {
        var existingCareer = await _careerService.GetCareerByIdAsync(id);
        if (existingCareer == null)
        {
            return NotFound();
        }

        await _careerService.DeleteCareerAsync(id);
        return NoContent();
    }
}