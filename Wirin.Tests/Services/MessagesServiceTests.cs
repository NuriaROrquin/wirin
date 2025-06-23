using Xunit;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using Wirin.Domain.Services;

public class MessageServiceTests
{
    private readonly Mock<IMessageRepository> _messageRepositoryMock;
    private readonly MessageService _messageService;

    public MessageServiceTests()
    {
        _messageRepositoryMock = new Mock<IMessageRepository>();
        _messageService = new MessageService(_messageRepositoryMock.Object);
    }

    [Fact]
    public async Task GetMessagesByUserIdAsync_ShouldReturnMessages()
    {
        // Arrange
        string userId = "user123";
        var expectedMessages = new List<Message>
        {
            new Message { id = 1, userFromId = userId, content = "Hola" },
            new Message { id = 2, userFromId = userId, content = "¿Cómo estás?" }
        };

        _messageRepositoryMock.Setup(repo => repo.GetMessagesByUserIdAsync(userId))
                              .ReturnsAsync(expectedMessages);

        // Act
        var result = await _messageService.GetMessagesByUserIdAsync(userId);

        // Assert
        Assert.Equal(expectedMessages.Count, result.Count);
        Assert.All(result, m => Assert.Equal(userId, m.userFromId));
    }

    [Fact]
    public async Task SendMessageAsync_ShouldCallSaveMessageAsync()
    {
        // Arrange
        var message = new Message { id = 1, userFromId = "user123", content = "Nuevo mensaje" };

        // Act
        await _messageService.SendMessageAsync(message);

        // Assert
        _messageRepositoryMock.Verify(repo => repo.SaveMessageAsync(message), Times.Once);
    }

    [Fact]
    public async Task SaveFile_ShouldSaveTheFileToGivenPath()
    {
        // Arrange
        var fileName = "test.txt";
        var content = "Archivo de prueba";
        var fileMock = new Mock<IFormFile>();
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(stream.Length);
        fileMock.Setup(f => f.CopyTo(It.IsAny<Stream>()))
                .Callback<Stream>(s => stream.CopyTo(s));

        var destinyFolder = Path.GetTempPath();
        var fullPath = Path.Combine(destinyFolder, fileName);

        // Act
        var resultPath = await _messageService.SaveFile(fileMock.Object, destinyFolder);

        // Assert
        Assert.Equal(fullPath, resultPath);
        Assert.True(File.Exists(resultPath));

        // Cleanup
        File.Delete(resultPath);
    }

    [Fact]
    public async Task GetFileByMessageIdAsync_ShouldReturnFileStream_WhenMessageHasFilePath()
    {
        // Arrange
        int messageId = 1;
        var fileName = "archivo.txt";
        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        var filePath = Path.Combine(uploadsPath, fileName);
        Directory.CreateDirectory(uploadsPath);
        await File.WriteAllTextAsync(filePath, "Contenido simulado");

        var message = new Message { id = messageId, filePath = fileName };

        _messageRepositoryMock.Setup(repo => repo.GetByIdAsync(messageId))
                              .ReturnsAsync(message);

        // Act
        var stream = await _messageService.GetFileByMessageIdAsync(messageId);

        // Assert
        Assert.NotNull(stream);
        stream.Dispose();

        // Cleanup
        File.Delete(filePath);
    }


    [Fact]
    public async Task UpdateAsync_ShouldCallUpdateMessageAsync()
    {
        // Arrange
        var message = new Message { id = 1, content = "Actualizado" };

        // Act
        await _messageService.UpdateAsync(message);

        // Assert
        _messageRepositoryMock.Verify(repo => repo.UpdateMessageAsync(message), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMessage_WhenFound()
    {
        // Arrange
        int id = 1;
        var message = new Message { id = id, content = "Encontrado" };

        _messageRepositoryMock.Setup(repo => repo.GetByIdAsync(id))
                              .ReturnsAsync(message);

        // Act
        var result = await _messageService.GetByIdAsync(id);

        // Assert
        Assert.Equal(message.id, result.id);
        Assert.Equal(message.content, result.content);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowException_WhenMessageNotFound()
    {
        // Arrange
        int id = 999;
        _messageRepositoryMock.Setup(repo => repo.GetByIdAsync(id))
                              .ReturnsAsync((Message)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _messageService.GetByIdAsync(id));
        Assert.Equal($"Mensaje con ID {id} no encontrado.", exception.Message);
    }
}
