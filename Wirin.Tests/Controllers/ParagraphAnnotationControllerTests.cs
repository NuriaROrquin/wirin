using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Wirin.Api.Controllers;
using Wirin.Domain.Services;
using Wirin.Domain.Dtos.Request;
using Wirin.Domain.Repositories;

public class ParagraphAnnotationControllerTests
{
    private readonly Mock<ParagraphAnnotationService> _serviceMock;
    private readonly ParagraphAnnotationController _controller;

    public ParagraphAnnotationControllerTests()
    {
        var mockOrderAnnotationsRepo = new Mock<IParagraphAnnotationRepository>();

        // Si el servicio tiene un constructor con parámetros, pásalos acá, o crea un constructor vacío para test.
        _serviceMock = new Mock<ParagraphAnnotationService>(mockOrderAnnotationsRepo.Object);

        _controller = new ParagraphAnnotationController(_serviceMock.Object);
    }

    [Fact]
    public async Task SaveParagraphAnnotationAsync_ReturnsOk_WhenServiceSucceeds()
    {
        var request = new OrderParagrapAnnotationsRequest();

        _serviceMock.Setup(s => s.SaveParagraphAnnotationAsync(request))
                    .Returns(Task.CompletedTask);

        var result = await _controller.SaveParagraphAnnotationAsync(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Paragraph annotation processed successfully.", okResult.Value);

        _serviceMock.Verify(s => s.SaveParagraphAnnotationAsync(request), Times.Once);
    }

    [Fact]
    public async Task SaveParagraphAnnotationAsync_ReturnsStatusCode500_WhenServiceThrows()
    {
        var request = new OrderParagrapAnnotationsRequest();
        var exceptionMessage = "Error inesperado";

        _serviceMock.Setup(s => s.SaveParagraphAnnotationAsync(request))
                    .ThrowsAsync(new Exception(exceptionMessage));

        var result = await _controller.SaveParagraphAnnotationAsync(request);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
        Assert.Equal($"Error al procesar la anotación: {exceptionMessage}", objectResult.Value);

        _serviceMock.Verify(s => s.SaveParagraphAnnotationAsync(request), Times.Once);
    }
}
