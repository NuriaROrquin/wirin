using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wirin.Api.Controllers;
using Wirin.Api.Dtos.Requests;
using Wirin.Api.Dtos.Response;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using Wirin.Domain.Services;
using Xunit;

namespace Wirin.Tests.Controllers;

public class SubjectControllerTests
{
    private readonly Mock<SubjectService> _mockSubjectService;
    private readonly Mock<CareerService> _mockCareerService;
    private readonly SubjectController _controller;

    public SubjectControllerTests()
    {
        var mockSubjectRepository = new Mock<ISubjectRepository>();
        var mockCareerRepository = new Mock<ICareerRepository>();

        _mockSubjectService = new Mock<SubjectService>(mockSubjectRepository.Object);
        _mockCareerService = new Mock<CareerService>(mockCareerRepository.Object);
        _controller = new SubjectController(_mockSubjectService.Object, _mockCareerService.Object);
    }

    private Subject CreateSampleSubject(int id, string name = "Test Subject", int careerId = 1)
    {
        return new Subject { Id = id, Name = name, CareerId = careerId, Career = new Career { Id = careerId, Name = "Test Career"}};
    }

    private Career CreateSampleCareer(int id, string name = "Test Career")
    {
        return new Career { Id = id, Name = name };
    }

    [Fact]
    public async Task GetAllSubjects_ReturnsOkObjectResult_WithListOfSubjectResponses()
    {
        var subjectsDomain = new List<Subject> { CreateSampleSubject(1), CreateSampleSubject(2, "Subject Two") };
        _mockSubjectService.Setup(s => s.GetAllSubjectsAsync()).ReturnsAsync(subjectsDomain);

        var result = await _controller.GetAllSubjects();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<SubjectResponse>>(okResult.Value);
        Assert.Equal(2, returnValue.Count);
        Assert.Equal("Test Subject", returnValue[0].Name);
    }

    [Fact]
    public async Task GetSubjectById_ExistingId_ReturnsOkObjectResult_WithSubjectResponse()
    {
        var subjectDomain = CreateSampleSubject(1);
        _mockSubjectService.Setup(s => s.GetSubjectByIdAsync(1)).ReturnsAsync(subjectDomain);

        var result = await _controller.GetSubjectById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<SubjectResponse>(okResult.Value);
        Assert.Equal(1, returnValue.Id);
        Assert.Equal("Test Subject", returnValue.Name);
    }


    [Fact]
    public async Task GetSubjectsByCareerId_ReturnsOkObjectResult_WithListOfSubjectResponses()
    {
        // Arrange
        var subjects = new List<Subject>
    {
        new Subject { Id = 1, Name = "Algebra", CareerId = 1 },
        new Subject { Id = 2, Name = "Historia", CareerId = 1 }
    };

        _mockSubjectService.Setup(s => s.GetSubjectsByCareerIdAsync(1)).ReturnsAsync(subjects);

        // Act
        var result = await _controller.GetSubjectsByCareerId(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsAssignableFrom<IEnumerable<SubjectResponse>>(okResult.Value);
        Assert.Equal(2, response.Count());
    }


    [Fact]
    public async Task CreateSubject_ValidModel_ReturnsCreatedAtActionResult_WithSubjectResponse()
    {
        var request = new CreateSubjectRequest { Name = "New Subject", CareerId = 1 };
        var careerDomain = CreateSampleCareer(1);
        _mockCareerService.Setup(s => s.GetCareerByIdAsync(1)).ReturnsAsync(careerDomain);
        
        _mockSubjectService.Setup(s => s.AddSubjectAsync(It.IsAny<Subject>()))
            .Callback<Subject>(sub => sub.Id = 1) // Simulate ID generation
            .Returns(Task.CompletedTask);

        // For CreatedAtAction's call to GetSubjectByIdAsync
        _mockSubjectService.Setup(s => s.GetSubjectByIdAsync(1)).Returns(Task.FromResult(new Subject { Id = 1, Name = request.Name,  CareerId = request.CareerId }));

        var result = await _controller.CreateSubject(request);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(SubjectController.GetSubjectById), createdAtActionResult.ActionName);
        Assert.Equal(1, createdAtActionResult.RouteValues["id"]);
        var returnValue = Assert.IsType<SubjectResponse>(createdAtActionResult.Value);
        Assert.Equal("New Subject", returnValue.Name);
        Assert.Equal(1, returnValue.Id);
    }

    [Fact]
    public async Task CreateSubject_InvalidCareerId_ReturnsBadRequestObjectResult()
    {
        var request = new CreateSubjectRequest { Name = "New Subject", CareerId = 99 };
        _mockCareerService.Setup(s => s.GetCareerByIdAsync(99)).ReturnsAsync((Career)null);

        var result = await _controller.CreateSubject(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsType<SerializableError>(badRequestResult.Value);
        Assert.True(errors.ContainsKey("CareerId"));
    }

    [Fact]
    public async Task UpdateSubject_ValidModel_ExistingId_ReturnsNoContentResult()
    {
        var request = new UpdateSubjectRequest { Name = "Updated Subject", CareerId = 1 };
        var existingSubject = CreateSampleSubject(1, "Old Name");
        var existingCareer = CreateSampleCareer(1);

        _mockSubjectService.Setup(s => s.GetSubjectByIdAsync(1)).Returns(Task.FromResult(existingSubject));
        _mockCareerService.Setup(s => s.GetCareerByIdAsync(1)).Returns(Task.FromResult(existingCareer)); // For CareerId validation
        _mockSubjectService.Setup(s => s.UpdateSubjectAsync(It.IsAny<Subject>())).Returns(Task.CompletedTask);

        var result = await _controller.UpdateSubject(1, request);

        Assert.IsType<NoContentResult>(result);
        _mockSubjectService.Verify(s => s.UpdateSubjectAsync(It.Is<Subject>(sub => sub.Id == 1 && sub.Name == "Updated Subject" && sub.CareerId == 1)), Times.Once);
    }

    [Fact]
    public async Task UpdateSubject_InvalidNewCareerId_ReturnsBadRequest()
    {
        var request = new UpdateSubjectRequest { Name = "Updated Subject", CareerId = 99 }; // Invalid new CareerId
        var existingSubject = CreateSampleSubject(1, "Old Name", careerId: 1);

        _mockSubjectService.Setup(s => s.GetSubjectByIdAsync(1)).Returns(Task.FromResult(existingSubject));
        _mockCareerService.Setup(s => s.GetCareerByIdAsync(99)).ReturnsAsync((Career)null); // New CareerId does not exist

        var result = await _controller.UpdateSubject(1, request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsType<SerializableError>(badRequestResult.Value);
        Assert.True(errors.ContainsKey("CareerId"));
    }

    [Fact]
    public async Task DeleteSubject_ExistingId_ReturnsNoContentResult()
    {
        var existingSubject = CreateSampleSubject(1);
        _mockSubjectService.Setup(s => s.GetSubjectByIdAsync(1)).Returns(Task.FromResult(existingSubject));
        _mockSubjectService.Setup(s => s.DeleteSubjectAsync(1)).Returns(Task.CompletedTask);

        var result = await _controller.DeleteSubject(1);

        Assert.IsType<NoContentResult>(result);
        _mockSubjectService.Verify(s => s.DeleteSubjectAsync(1), Times.Once);
    }
}