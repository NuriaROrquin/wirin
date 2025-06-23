using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wirin.Api.Controllers;
using Wirin.Domain.Services;
using Wirin.Domain.Models;
using Wirin.Api.Dtos.Requests;
using Wirin.Api.Dtos.Response;
using System.Text;
using Wirin.Domain.Repositories;
using System.Security.Claims;

public class MessageControllerTests
{
    private readonly Mock<IMessageRepository> _messageRepositoryMock;
    private readonly Mock<UserService> _userServiceMock;
    private readonly Mock<MessageService> _messageServiceMock;
    private readonly MessageController _controller;

    public MessageControllerTests()
    {
        _messageRepositoryMock = new Mock<IMessageRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        _userServiceMock = new Mock<UserService>(userRepositoryMock.Object);
        _messageServiceMock = new Mock<MessageService>(_messageRepositoryMock.Object);
        _controller = new MessageController(_messageServiceMock.Object, _userServiceMock.Object);
    }

    [Fact]
    public async Task GetMessagesAsync_ReturnsOkWithMessages()
    {
        var userId = "user1";
        var messages = new List<Message> { new Message { id = 1, subject = "Test", userFromId = userId } };

        _userServiceMock.Setup(u => u.GetUserTrasabilityId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
        _messageServiceMock.Setup(s => s.GetMessagesAsync(userId)).ReturnsAsync(messages);

        var result = await _controller.GetMessagesAsync();

        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsAssignableFrom<List<MessageResponse>>(ok.Value);
        Assert.Single(list);
        Assert.Equal(messages[0].id, list[0].id);
    }

    [Fact]
    public async Task GetMessagesAsync_ReturnsNotFoundWhenNoMessages()
    {
        var userId = "user1";

        _userServiceMock.Setup(u => u.GetUserTrasabilityId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
        _messageServiceMock.Setup(s => s.GetMessagesAsync(userId)).ReturnsAsync(new List<Message>());

        var result = await _controller.GetMessagesAsync();

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No se encontraron mensajes.", notFound.Value);
    }

    [Fact]
    public async Task GetSendedMessagesAsync_ReturnsOkWithMessages()
    {
        var userId = "user123";
        var messages = new List<Message> { new Message { id = 1, subject = "Test", userFromId = userId } };

        _userServiceMock.Setup(u => u.GetUserTrasabilityId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
        _messageServiceMock.Setup(s => s.GetMessagesAsync(userId)).ReturnsAsync(messages);

        var result = await _controller.GetSendedMessagesAsync();

        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsAssignableFrom<List<MessageResponse>>(ok.Value);
        Assert.Single(list);
    }

    [Fact]
    public async Task GetRevisedMessagesAsync_ReturnsOkWithMessages()
    {
        var userId = "user123";
        var messages = new List<Message> { new Message { id = 2, subject = "Recibido", userToId = userId } };

        _userServiceMock.Setup(u => u.GetUserTrasabilityId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
        _messageServiceMock.Setup(s => s.GetRecivedMessagesAsync(userId)).ReturnsAsync(messages);

        var result = await _controller.GetRevisedMessagesAsync();

        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsAssignableFrom<List<MessageResponse>>(ok.Value);
        Assert.Single(list);
    }

    [Fact]
    public async Task SendMessageWithFileAsync_UploadsFileAndSavesMessage()
    {
        var msgRequest = new MessageRequest { id = 1, subject = "Hi" };
        var fileMock = new Mock<IFormFile>();
        var userId = "user1";

        _userServiceMock.Setup(u => u.GetUserTrasabilityId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
        _messageServiceMock.Setup(s => s.SaveFile(It.IsAny<IFormFile>(), It.IsAny<string>())).ReturnsAsync("fakepath");
        _messageServiceMock.Setup(s => s.SendMessageAsync(It.IsAny<Message>())).Returns(Task.CompletedTask);

        var result = await _controller.SendMessageWithFileAsync(msgRequest, fileMock.Object);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Mensaje enviado correctamente.", ok.Value);
    }

    [Fact]
    public async Task UpdateMessageWithFileAsync_UpdatesWithFile()
    {
        var msgRequest = new MessageRequest { id = 1, subject = "Update" };
        var fileMock = new Mock<IFormFile>();

        _messageServiceMock.Setup(s => s.SaveFile(It.IsAny<IFormFile>(), It.IsAny<string>()))
                           .ReturnsAsync("path/to/file");

        _messageServiceMock.Setup(s => s.UpdateAsync(It.IsAny<Message>()))
                           .Returns(Task.CompletedTask);

        var result = await _controller.UpdateMessageWithFileAsync(msgRequest, fileMock.Object);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Mensaje actualizado correctamente.", ok.Value);
    }

    [Fact]
    public async Task UpdateMessageWithFileAsync_UsesExistingFilePath_WhenFileIsNull()
    {
        var msgRequest = new MessageRequest { id = 1, subject = "Update" };
        var existingMessage = new Message { filePath = "existing/path" };

        _messageServiceMock.Setup(s => s.GetByIdAsync(msgRequest.id)).ReturnsAsync(existingMessage);
        _messageServiceMock.Setup(s => s.UpdateAsync(It.Is<Message>(m => m.filePath == "existing/path")))
                           .Returns(Task.CompletedTask);

        var result = await _controller.UpdateMessageWithFileAsync(msgRequest, null);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Mensaje actualizado correctamente.", ok.Value);
    }
}