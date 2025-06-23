using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using Wirin.Domain.Services;
using Wirin.Api.Dtos.Response;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IOrderTrasabilityRepository> _orderTrasabilityRepositoryMock;
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        var mockOrderSequenceRepo = new Mock<IOrderSequenceRepository>();

        _orderTrasabilityRepositoryMock = new Mock<IOrderTrasabilityRepository>();
        _service = new OrderService(_orderRepositoryMock.Object, _orderTrasabilityRepositoryMock.Object, mockOrderSequenceRepo.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsListOfOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order { Id = 1, CreationDate = DateTime.Now },
            new Order { Id = 2, CreationDate = DateTime.Now }
        };
        _orderRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(orders);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, ((List<Order>)result).Count);
        _orderRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsOrder()
    {
        // Arrange
        int orderId = 1;
        var order = new Order { Id = orderId, CreationDate = DateTime.Now };
        _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(orderId)).ReturnsAsync(order);

        // Act
        var result = await _service.GetByIdAsync(orderId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orderId, result.Id);
        _orderRepositoryMock.Verify(r => r.GetByIdAsync(orderId), Times.Once);
    }

    [Fact]
    public async Task AddAsync_SavesOrderAndOrderTrasability()
    {
        // Arrange
        var order = new Order();
        string path = "path/to/file";
        string userId = "user123";

        _orderRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask)
            .Callback<Order>(o => o.Id = 5);  // Simula que después de guardar se asigna un Id

        _orderTrasabilityRepositoryMock
            .Setup(repo => repo.SaveAsync(It.IsAny<OrderTrasability>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.AddAsync(order, path, userId);

        // Assert
        Assert.Equal(path, order.FilePath);
        Assert.NotEqual(default(DateTime), order.CreationDate);
        _orderRepositoryMock.Verify(r => r.AddAsync(order), Times.Once);
        _orderTrasabilityRepositoryMock.Verify(r => r.SaveAsync(It.Is<OrderTrasability>(
            ot => ot.OrderId == order.Id &&
                  ot.UserId == userId &&
                  ot.Action == "Creado")), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesOrderAndSavesOrderTrasability()
    {
        // Arrange
        var order = new Order { Id = 1, FilePath = "oldPath" };
        string newPath = "new/path";
        string userId = "user456";

        _orderRepositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);

        _orderTrasabilityRepositoryMock
            .Setup(repo => repo.SaveAsync(It.IsAny<OrderTrasability>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(order, newPath, userId);

        // Assert
        Assert.Equal(newPath, order.FilePath);
        _orderRepositoryMock.Verify(r => r.UpdateAsync(order), Times.Once);
        _orderTrasabilityRepositoryMock.Verify(r => r.SaveAsync(It.Is<OrderTrasability>(
            ot => ot.OrderId == order.Id &&
                  ot.UserId == userId &&
                  ot.Action == "Actualizado")), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_CallsDeleteOnRepository()
    {
        // Arrange
        int orderId = 3;
        _orderRepositoryMock
            .Setup(repo => repo.DeleteAsync(orderId))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(orderId);

        // Assert
        _orderRepositoryMock.Verify(r => r.DeleteAsync(orderId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsException_WhenRepositoryThrows()
    {
        // Arrange
        int orderId = 7;
        _orderRepositoryMock
            .Setup(repo => repo.DeleteAsync(orderId))
            .ThrowsAsync(new Exception("Error deleting order"));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _service.DeleteAsync(orderId));
        Assert.Equal("Error deleting order", ex.Message);
    }
    [Fact]
    public async Task GetFileByOrderIdAsync_ReturnsFileStream_WhenFilePathExists()
    {
        // Arrange
        int orderId = 1;
        string fileName = "testfile.txt";
        var order = new Order { Id = orderId, FilePath = fileName };

        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        Directory.CreateDirectory(uploadsFolder); // Crear carpeta si no existe
        string fullPath = Path.Combine(uploadsFolder, fileName);

        // Crear archivo dummy
        await File.WriteAllTextAsync(fullPath, "Contenido de prueba");

        try
        {
            // Act
            var result = await _service.GetFileByOrderIdAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<FileStream>(result);

            result.Dispose();
        }
        finally
        {
            // Limpiar archivo creado para no ensuciar el entorno de tests
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }


    [Fact]
    public async Task GetFileByOrderIdAsync_ReturnsNull_WhenFilePathIsNull()
    {
        // Arrange
        int orderId = 2;
        var order = new Order { Id = orderId, FilePath = null };
        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        // Act
        var result = await _service.GetFileByOrderIdAsync(orderId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SaveFile_ReturnsFilePath()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        string fileName = "file.txt";
        string destinyFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        fileMock.Setup(f => f.FileName).Returns(fileName);

        var content = "Hello World from a fake file";
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;

        fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
        fileMock.Setup(f => f.CopyTo(It.IsAny<Stream>())).Callback<Stream>(stream =>
        {
            ms.CopyTo(stream);
        });

        // Act
        var result = await _service.SaveFile(fileMock.Object, destinyFolder);

        // Assert
        Assert.Equal(Path.Combine(destinyFolder, fileName), result);
    }
}
