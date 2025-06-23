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
public class SubjectController : ControllerBase
{
    private readonly SubjectService _subjectService;
    private readonly CareerService _careerService; // To validate CareerId

    public SubjectController(SubjectService subjectService, CareerService careerService)
    {
        _subjectService = subjectService;
        _careerService = careerService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SubjectResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllSubjects()
    {
        var subjects = await _subjectService.GetAllSubjectsAsync();
        return Ok(SubjectResponse.ListFromDomain(subjects));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SubjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSubjectById(int id)
    {
        var subject = await _subjectService.GetSubjectByIdAsync(id);
        if (subject == null)
        {
            return NotFound();
        }
        return Ok(SubjectResponse.FromDomain(subject));
    }

    [HttpGet("career/{careerId}")]
    [ProducesResponseType(typeof(IEnumerable<SubjectResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubjectsByCareerId(int careerId)
    {
        var subjects = await _subjectService.GetSubjectsByCareerIdAsync(careerId);
        return Ok(SubjectResponse.ListFromDomain(subjects));
    }

    [HttpPost]
    [ProducesResponseType(typeof(SubjectResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validate if CareerId exists
        var career = await _careerService.GetCareerByIdAsync(request.CareerId);
        if (career == null)
        {
            ModelState.AddModelError("CareerId", "Invalid CareerId.");
            return BadRequest(ModelState);
        }

        var subject = new Subject
        {
            Name = request.Name,
            CareerId = request.CareerId
        };

        await _subjectService.AddSubjectAsync(subject);
        // It's good practice to load the created subject with its navigation properties if needed for the response
        var createdSubject = await _subjectService.GetSubjectByIdAsync(subject.Id);
        return CreatedAtAction(nameof(GetSubjectById), new { id = subject.Id }, SubjectResponse.FromDomain(createdSubject));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSubject(int id, [FromBody] UpdateSubjectRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingSubject = await _subjectService.GetSubjectByIdAsync(id);
        if (existingSubject == null)
        {
            return NotFound();
        }

        // Validate if new CareerId exists
        if (existingSubject.CareerId != request.CareerId)
        {
            var career = await _careerService.GetCareerByIdAsync(request.CareerId);
            if (career == null)
            {
                ModelState.AddModelError("CareerId", "Invalid CareerId.");
                return BadRequest(ModelState);
            }
        }

        existingSubject.Name = request.Name;
        existingSubject.CareerId = request.CareerId;

        await _subjectService.UpdateSubjectAsync(existingSubject);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSubject(int id)
    {
        var existingSubject = await _subjectService.GetSubjectByIdAsync(id);
        if (existingSubject == null)
        {
            return NotFound();
        }

        await _subjectService.DeleteSubjectAsync(id);
        return NoContent();
    }
}