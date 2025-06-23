using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Wirin.Api.Controllers;
using Wirin.Domain.Services;
using Wirin.Domain.Repositories;
using Wirin.Application;
using Wirin.Domain.Dtos.OCR;
using Microsoft.AspNetCore.Http;
using Wirin.Domain.Providers;
using Wirin.Infrastructure.Services;
using Wirin.Infrastructure.Repositories;

public class OcrControllerTests
{
    private readonly Mock<ProcessWithLocalOcrUseCase> _mockOcrUseCase;
    private readonly Mock<UserService> _mockUserService;
    private readonly OcrController _controller;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    public OcrControllerTests()
    {// Dependencias requeridas por el constructor
        var mockOcrProvider = new Mock<IOcrProvider>();
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockOrderParagraphRepo = new Mock<IOrderParagraphRepository>();
        var mockOrderAnnotationsRepo = new Mock<IParagraphAnnotationRepository>();
        var mockOrderManagmentRepo = new Mock<IOrderManagmentRepository>();
        var mockOrderTrasabilitiRepo = new Mock<IOrderTrasabilityRepository>();
        var mockOrderDeliveryRepo = new Mock<IOrderDeliveryRepository>();
        var mockOrderSequenceRepo = new Mock<IOrderSequenceRepository>();

    // Servicios que usan los repositorios
    var orderService = new OrderService(mockOrderRepo.Object, mockOrderTrasabilitiRepo.Object, mockOrderSequenceRepo.Object);
        var orderParagraphService = new OrderParagraphService(mockOrderParagraphRepo.Object, mockOrderAnnotationsRepo.Object, mockOrderTrasabilitiRepo.Object); // podés pasar null si no usás ese repo
        var orderManagmentService = new OrderManagmentService(mockOrderParagraphRepo.Object,mockOrderManagmentRepo.Object, mockOrderRepo.Object, mockOrderDeliveryRepo.Object, mockOrderSequenceRepo.Object, mockOrderTrasabilitiRepo.Object);

        _mockOcrUseCase = new Mock<ProcessWithLocalOcrUseCase>(
            mockOcrProvider.Object,
            orderService,
            orderParagraphService,
            orderManagmentService
        );

        _userRepositoryMock = new Mock<IUserRepository>();


        _mockUserService = new Mock<UserService>(_userRepositoryMock.Object);
        _controller = new OcrController(_mockOcrUseCase.Object, _mockUserService.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "testUserId")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task ProcessOcr_ReturnsOk_WhenResultIsNotNull()
    {
        // Arrange
        var engine = "local";
        var id = 123;
        var userTrasabilityId = "user123";

        var expectedResult = new OcrResultDto
        {
            Status = "éxito",
            Message = "PDF procesado correctamente con Tesseract OCR.",
            FullText = "Texto completo OCR",
            Metadata = new OcrMetadataDto(), // si tiene propiedades, rellenar aquí
            Pages = new List<OcrPageResultDto>() // lista vacía o con páginas de prueba
        };

        _mockUserService.Setup(u => u.GetUserTrasabilityId(It.IsAny<ClaimsPrincipal>()))
            .Returns(userTrasabilityId);

        _mockOcrUseCase.Setup(u => u.__Invoke(engine, id, userTrasabilityId))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.ProcessOcr(engine, id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedResult, okResult.Value);
    }

    [Fact]
    public async Task ProcessOcr_ReturnsNotFound_WhenResultIsNull()
    {
        // Arrange
        var engine = "local";
        var id = 123;
        var userTrasabilityId = "user123";

        _mockUserService.Setup(u => u.GetUserTrasabilityId(It.IsAny<ClaimsPrincipal>()))
            .Returns(userTrasabilityId);

        _mockOcrUseCase.Setup(u => u.__Invoke(engine, id, userTrasabilityId))
            .ReturnsAsync((OcrResultDto)null);

        // Act
        var result = await _controller.ProcessOcr(engine, id);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("No se encontraron resultados para el OCR", notFoundResult.Value.ToString());
    }

    [Fact]
    public async Task ProcessOcr_Returns500_WhenExceptionThrown()
    {
        // Arrange
        var engine = "local";
        var id = 123;
        var userTrasabilityId = "user123";
        var exceptionMessage = "Error inesperado";

        _mockUserService.Setup(u => u.GetUserTrasabilityId(It.IsAny<ClaimsPrincipal>()))
            .Returns(userTrasabilityId);

        _mockOcrUseCase.Setup(u => u.__Invoke(engine, id, userTrasabilityId))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _controller.ProcessOcr(engine, id);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);

        var json = System.Text.Json.JsonSerializer.Serialize(objectResult.Value);
        var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);

        Assert.NotNull(dict);
        Assert.True(dict.ContainsKey("error"));
        Assert.Equal(exceptionMessage, dict["error"]);
    }


}
