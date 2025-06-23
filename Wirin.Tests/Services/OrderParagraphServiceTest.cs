using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Wirin.Api.Dtos.Response;
using Wirin.Domain.Dtos.OCR;
using Wirin.Domain.Dtos.Request;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using Wirin.Domain.Services;
using Wirin.Infrastructure.Dtos.Requests;
using Xunit;

public class OrderParagraphServiceTests
{
    private readonly Mock<IOrderParagraphRepository> _orderParagraphRepositoryMock;
    private readonly Mock<IParagraphAnnotationRepository> _paragraphAnnotationRepositoryMock;
    private readonly Mock<IOrderTrasabilityRepository> _orderTrasabilityRepositoryMock;

    private readonly OrderParagraphService _sut;

    public OrderParagraphServiceTests()
    {
        _orderParagraphRepositoryMock = new Mock<IOrderParagraphRepository>();
        _paragraphAnnotationRepositoryMock = new Mock<IParagraphAnnotationRepository>();
        _orderTrasabilityRepositoryMock = new Mock<IOrderTrasabilityRepository>();

        // OrderService no se usa realmente dentro del constructor, así que se pasa null
        _sut = new OrderParagraphService(
            _orderParagraphRepositoryMock.Object,
            _paragraphAnnotationRepositoryMock.Object,
            _orderTrasabilityRepositoryMock.Object);
    }

    [Fact]
    public async Task UpdateParagraphAsync_UpdatesParagraphAndSavesTrasability()
    {
        var request = new SaveParagraphRequest
        {
            orderId = 1,
            pageNumber = 2,
            paragraphText = "Nuevo texto",
            hasError = true
        };
        var existingParagraph = new Paragraph
        {
            OrderId = 1,
            PageNumber = 2,
            ParagraphText = "Texto viejo",
            HasError = false
        };

        _orderParagraphRepositoryMock
            .Setup(x => x.GetParagraphByIdAsync(request.orderId, request.pageNumber))
            .ReturnsAsync(existingParagraph);

        _orderParagraphRepositoryMock
            .Setup(x => x.UpdateParagraphAsync(It.IsAny<Paragraph>()))
            .Returns(Task.CompletedTask);

        _orderTrasabilityRepositoryMock
            .Setup(x => x.SaveAsync(It.IsAny<OrderTrasability>()))
            .Returns(Task.CompletedTask);

        await _sut.UpdateParagraphAsync(request, "user1", "user2");

        _orderParagraphRepositoryMock.Verify(x => x.GetParagraphByIdAsync(request.orderId, request.pageNumber), Times.Once);
        _orderParagraphRepositoryMock.Verify(x => x.UpdateParagraphAsync(It.Is<Paragraph>(p =>
            p.ParagraphText == request.paragraphText &&
            p.HasError == request.hasError)), Times.Once);
        _orderTrasabilityRepositoryMock.Verify(x => x.SaveAsync(It.Is<OrderTrasability>(t =>
            t.OrderId == request.orderId &&
            t.UserId == "user1" &&
            t.Action == "UpdateParagraph")), Times.Once);
    }

    [Fact]
    public async Task SaveParagraphsAsync_CreatesOrUpdatesParagraphsAndSavesTrasability()
    {
        var orderId = 1;
        var trasabilityUserId = "userX";
        var ocrPages = new List<OcrPageResultDto>
        {
            new() { Number = 1, Text = "Texto 1", Confidence = 0.9f },
            new() { Number = 2, Text = "Texto 2", Confidence = 0.8f }
        };

        // Para el primer párrafo no existe, para el segundo sí
        _orderParagraphRepositoryMock
            .Setup(x => x.GetParagraphByIdAsync(orderId, 1))
            .ReturnsAsync((Paragraph)null);

        _orderParagraphRepositoryMock
            .Setup(x => x.GetParagraphByIdAsync(orderId, 2))
            .ReturnsAsync(new Paragraph
            {
                OrderId = orderId,
                PageNumber = 2,
                ParagraphText = "Texto viejo",
                Confidence = 0.7f
            });

        _orderParagraphRepositoryMock
            .Setup(x => x.SaveAsync(It.IsAny<Paragraph>()))
            .Returns(Task.CompletedTask);

        _orderParagraphRepositoryMock
            .Setup(x => x.UpdateParagraphAsync(It.IsAny<Paragraph>()))
            .Returns(Task.CompletedTask);

        _orderTrasabilityRepositoryMock
            .Setup(x => x.SaveAsync(It.IsAny<OrderTrasability>()))
            .Returns(Task.CompletedTask);

        await _sut.SaveParagraphsAsync(ocrPages, orderId, trasabilityUserId);

        // Verifica que haya llamado SaveAsync para el nuevo párrafo
        _orderParagraphRepositoryMock.Verify(x => x.SaveAsync(It.Is<Paragraph>(p =>
            p.PageNumber == 1 && p.ParagraphText == "Texto 1")), Times.Once);

        // Verifica que haya llamado UpdateParagraphAsync para el existente
        _orderParagraphRepositoryMock.Verify(x => x.UpdateParagraphAsync(It.Is<Paragraph>(p =>
            p.PageNumber == 2 && p.ParagraphText == "Texto 2")), Times.Once);

        // Verifica que haya guardado la trazabilidad dos veces (uno para save y otro para update)
        _orderTrasabilityRepositoryMock.Verify(x => x.SaveAsync(It.Is<OrderTrasability>(t =>
            t.OrderId == orderId && t.UserId == trasabilityUserId &&
            (t.Action == "SaveParagraph" || t.Action == "UpdateParagraph"))), Times.Exactly(2));
    }




    [Fact]
    public async Task GetAllParagraphByOrderIdsAsync_ReturnsOcrResultDto()
    {
        var orderId = 7;
        var paragraphs = new List<Paragraph>
        {
            new() { OrderId = orderId, PageNumber = 1, ParagraphText = "Texto1", Confidence = 0.9f },
            new() { OrderId = orderId, PageNumber = 2, ParagraphText = "Texto2", Confidence = 0.8f }
        };

        var annotationsPage1 = new List<ParagraphAnnotation>
        {
            new() { AnnotationText = "Anot1" }
        };

        var annotationsPage2 = new List<ParagraphAnnotation>
        {
            new() { AnnotationText = "Anot2" },
            new() { AnnotationText = "Anot3" }
        };

        _orderParagraphRepositoryMock
            .Setup(x => x.GetAllParagraphsByOrderIdAsync(orderId))
            .ReturnsAsync(paragraphs);

        _paragraphAnnotationRepositoryMock
            .Setup(x => x.GetAllParagraphAnnotationsByParagraphIdAsync(orderId, 1))
            .ReturnsAsync(annotationsPage1);

        _paragraphAnnotationRepositoryMock
            .Setup(x => x.GetAllParagraphAnnotationsByParagraphIdAsync(orderId, 2))
            .ReturnsAsync(annotationsPage2);

        var result = await _sut.GetAllParagraphByOrderIdsAsync(orderId);

        Assert.Equal(2, result.Pages.Count);
        Assert.Equal(2, result.Metadata.TotalPages);
        Assert.Contains("Texto1 Texto2", result.FullText);
        Assert.Equal(annotationsPage1, result.Pages[0].Annotations);
        Assert.Equal(annotationsPage2, result.Pages[1].Annotations);
    }

    [Fact]
    public async Task GetParagraphsByOrderIdAsyncForMobil_ReturnsParagraphResponses()
    {
        var orderId = 5;
        var paragraphs = new List<Paragraph>
        {
            new() { OrderId = orderId, ParagraphText = "Texto A", PageNumber = 1 },
            new() { OrderId = orderId, ParagraphText = "Texto B", PageNumber = 2 }
        };

        _orderParagraphRepositoryMock
            .Setup(x => x.GetAllParagraphsByOrderIdAsync(orderId))
            .ReturnsAsync(paragraphs);

        var result = await _sut.GetParagraphsByOrderIdAsync(orderId);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.ParagraphText == "Texto A" && r.PageNumber == 1);
        Assert.Contains(result, r => r.ParagraphText == "Texto B" && r.PageNumber == 2);
    }

    [Fact]
    public async Task GetParagraphsByOrderIdAsyncForMobil_ReturnsEmptyList_WhenNoParagraphs()
    {
        var orderId = 10;
        _orderParagraphRepositoryMock
            .Setup(x => x.GetAllParagraphsByOrderIdAsync(orderId))
            .ReturnsAsync(new List<Paragraph>());

        var result = await _sut.GetParagraphsByOrderIdAsync(orderId);

        Assert.Empty(result);

    }
}