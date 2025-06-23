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

public class CareerControllerTests
{
    private readonly Mock<CareerService> _mockCareerService;
    private readonly CareerController _controller;

    public CareerControllerTests()
    {
        // CareerService depends on ICareerRepository, so we mock CareerService directly
        // If CareerService had complex logic, we might mock its dependencies instead.
        var mockcareerRepository = new Mock<ICareerRepository>();

        _mockCareerService = new Mock<CareerService>(mockcareerRepository.Object); 
        _controller = new CareerController(_mockCareerService.Object);
    }

    private Career CreateSampleCareer(int id, string name = "Test Career")
    {
        return new Career { Id = id, Name = name, CodDepartamento = "Test CodDepartamento", Subjects = new List<Subject>() };
    }

    [Fact]
    public async Task GetAllCareers_ReturnsOkObjectResult_WithListOfCareerResponses()
    {
        var careersDomain = new List<Career> { CreateSampleCareer(1), CreateSampleCareer(2, "Career Two") };
        _mockCareerService.Setup(s => s.GetAllCareersAsync()).ReturnsAsync(careersDomain);

        var result = await _controller.GetAllCareers();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<CareerResponse>>(okResult.Value);
        Assert.Equal(2, returnValue.Count);
        Assert.Equal("Test Career", returnValue[0].Name);
    }

    [Fact]
    public async Task GetCareerById_ExistingId_ReturnsOkObjectResult_WithCareerResponse()
    {
        var careerDomain = CreateSampleCareer(1);
        _mockCareerService.Setup(s => s.GetCareerByIdAsync(1)).ReturnsAsync(careerDomain);

        var result = await _controller.GetCareerById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<CareerResponse>(okResult.Value);
        Assert.Equal(1, returnValue.Id);
        Assert.Equal("Test Career", returnValue.Name);
    }

    [Fact]
    public async Task GetCareerById_NonExistingId_ReturnsNotFoundResult()
    {
        _mockCareerService.Setup(s => s.GetCareerByIdAsync(99)).ReturnsAsync((Career)null);

        var result = await _controller.GetCareerById(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateCareer_ValidModel_ReturnsCreatedAtActionResult_WithCareerResponse()
    {
        var request = new CreateCareerRequest { Name = "New Career", CodDepartamento = "New CodDep" };
        var careerToCreate = new Career { Name = request.Name, CodDepartamento = request.CodDepartamento };
        // Mock AddCareerAsync to assign an ID to the career object passed to it
        _mockCareerService.Setup(s => s.AddCareerAsync(It.IsAny<Career>()))
            .Callback<Career>(c => c.Id = 1) // Simulate ID generation
            .Returns(Task.CompletedTask);
        
        // When GetCareerByIdAsync is called by CreatedAtAction, return the career with ID
        _mockCareerService.Setup(s => s.GetCareerByIdAsync(1)).Returns(Task.FromResult(new Career { Id = 1, Name = request.Name, CodDepartamento = request.CodDepartamento }));

        var result = await _controller.CreateCareer(request);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(CareerController.GetCareerById), createdAtActionResult.ActionName);
        Assert.Equal(1, createdAtActionResult.RouteValues["id"]);
        var returnValue = Assert.IsType<CareerResponse>(createdAtActionResult.Value);
        Assert.Equal("New Career", returnValue.Name);
        Assert.Equal(1, returnValue.Id);
    }

    [Fact]
    public async Task CreateCareer_InvalidModel_ReturnsBadRequestObjectResult()
    {
        var request = new CreateCareerRequest { Name = null }; // Invalid
        _controller.ModelState.AddModelError("Name", "Required");

        var result = await _controller.CreateCareer(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<SerializableError>(badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateCareer_ValidModel_ExistingId_ReturnsNoContentResult()
    {
        var request = new UpdateCareerRequest { Name = "Updated Career", CodDepartamento = "Updated CodDep" };
        var existingCareer = CreateSampleCareer(1, "Old Name"); 
        _mockCareerService.Setup(s => s.GetCareerByIdAsync(1)).ReturnsAsync(existingCareer);
        _mockCareerService.Setup(s => s.UpdateCareerAsync(It.IsAny<Career>())).Returns(Task.CompletedTask);

        var result = await _controller.UpdateCareer(1, request);

        Assert.IsType<NoContentResult>(result);
        _mockCareerService.Verify(s => s.UpdateCareerAsync(It.Is<Career>(c => c.Id == 1 && c.Name == "Updated Career" && c.CodDepartamento == "Updated CodDep")), Times.Once);
    }

    [Fact]
    public async Task UpdateCareer_NonExistingId_ReturnsNotFoundResult()
    {
        var request = new UpdateCareerRequest { Name = "Updated" };
        _mockCareerService.Setup(s => s.GetCareerByIdAsync(99)).ReturnsAsync((Career)null);

        var result = await _controller.UpdateCareer(99, request);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteCareer_ExistingId_ReturnsNoContentResult()
    {
        var existingCareer = CreateSampleCareer(1);
        _mockCareerService.Setup(s => s.GetCareerByIdAsync(1)).ReturnsAsync(existingCareer);
        _mockCareerService.Setup(s => s.DeleteCareerAsync(1)).Returns(Task.CompletedTask);

        var result = await _controller.DeleteCareer(1);

        Assert.IsType<NoContentResult>(result);
        _mockCareerService.Verify(s => s.DeleteCareerAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteCareer_NonExistingId_ReturnsNotFoundResult()
    {
        _mockCareerService.Setup(s => s.GetCareerByIdAsync(99)).ReturnsAsync((Career)null);

        var result = await _controller.DeleteCareer(99);

        Assert.IsType<NotFoundResult>(result);
    }
}