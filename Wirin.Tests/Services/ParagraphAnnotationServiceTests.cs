using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Wirin.Domain.Dtos.Request;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using Wirin.Domain.Services;

public class ParagraphAnnotationServiceTests
{
    private readonly Mock<IParagraphAnnotationRepository> _paragraphAnnotationRepositoryMock;
    private readonly ParagraphAnnotationService _service;

    public ParagraphAnnotationServiceTests()
    {
        _paragraphAnnotationRepositoryMock = new Mock<IParagraphAnnotationRepository>();
        _service = new ParagraphAnnotationService(_paragraphAnnotationRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAnnotationsByParagraphIdAsync_ReturnsAnnotations()
    {
        // Arrange
        var paragraphId = 5;
        var request = new OrderParagrapAnnotationsRequest
        {
            ParagraphAnnotation = new ParagraphAnnotation { ParagraphId = paragraphId }
        };

        var expectedAnnotations = new List<ParagraphAnnotation>
        {
            new ParagraphAnnotation { ParagraphId = paragraphId, AnnotationText = "Error 1" },
            new ParagraphAnnotation { ParagraphId = paragraphId, AnnotationText = "Error 2" }
        };

        _paragraphAnnotationRepositoryMock
            .Setup(repo => repo.GetAllParagraphAnnotationsByParagraphIdAsync(paragraphId, paragraphId))
            .ReturnsAsync(expectedAnnotations);

        // Act
        var result = await _service.GetAllAnnotationsByParagraphIdAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedAnnotations.Count, result.Count);
        Assert.Equal(expectedAnnotations[0].AnnotationText, result[0].AnnotationText);
    }

    [Fact]
    public async Task SaveParagraphAnnotationAsync_CallsRepositorySave()
    {
        // Arrange
        var annotation = new ParagraphAnnotation { ParagraphId = 5, AnnotationText = "Error" };
        var request = new OrderParagrapAnnotationsRequest { ParagraphAnnotation = annotation };

        _paragraphAnnotationRepositoryMock
            .Setup(repo => repo.SaveParagraphAnnotationAsync(annotation))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _service.SaveParagraphAnnotationAsync(request);

        // Assert
        _paragraphAnnotationRepositoryMock.Verify(repo => repo.SaveParagraphAnnotationAsync(annotation), Times.Once);
    }
}
