using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wirin.Domain.Models;
using Wirin.Infrastructure.Context;
using Wirin.Infrastructure.Repositories;
using Wirin.Infrastructure.Transformers;
using Xunit;


    public class MessageRepositoryTests
    {
        private WirinDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<WirinDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // DB nueva por test
                .Options;

            return new WirinDbContext(options);
        }

        private Message CreateSampleMessage(int id = 1, string studentId = "student123")
        {
            return new Message
            {
                id = id,
                isDraft = false,
                userFromId = "user1",
                userToId = studentId,
                sender = "Profesor X",
                subject = "Asunto de prueba",
                date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                content = "Contenido del mensaje",
                filePath = null,
                responded = false,
                responseText = null
            };
        }

        [Fact]
        public async Task SaveMessageAsync_Should_Save_Message()
        {
            var context = GetInMemoryDbContext();
            var repo = new MessageRepository(context);
            var message = CreateSampleMessage();

            await repo.SaveMessageAsync(message);

            var saved = await context.Message.FirstOrDefaultAsync();
            Assert.NotNull(saved);
            Assert.Equal(message.subject, saved.subject);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Message()
        {
            var context = GetInMemoryDbContext();
            var original = CreateSampleMessage();
            context.Message.Add(MessageTransformer.ToEntity(original));
            await context.SaveChangesAsync();

            var repo = new MessageRepository(context);
            var result = await repo.GetByIdAsync(original.id);

            Assert.NotNull(result);
            Assert.Equal(original.subject, result.subject);
        }



    [Fact]
        public async Task GetMessagesByUserIdAsync_Should_Return_Filtered_Messages()
        {
            var context = GetInMemoryDbContext();
            context.Message.AddRange(
                MessageTransformer.ToEntity(CreateSampleMessage(1, "user1")),
                MessageTransformer.ToEntity(CreateSampleMessage(2, "user2"))
            );
            await context.SaveChangesAsync();

            var repo = new MessageRepository(context);
            var result = await repo.GetMessagesByUserIdAsync("user1");

            Assert.Single(result);
            Assert.Equal("user1", result[0].userToId);
        }

        [Fact]
        public async Task UpdateMessageAsync_Should_Modify_Message()
        {
            var context = GetInMemoryDbContext();
            var original = CreateSampleMessage();
            context.Message.Add(MessageTransformer.ToEntity(original));
            await context.SaveChangesAsync();

            var repo = new MessageRepository(context);
            original.subject = "Nuevo asunto";

            await repo.UpdateMessageAsync(original);

            var updated = await context.Message.FindAsync(original.id);
            Assert.Equal("Nuevo asunto", updated.subject);
        }

        [Fact]
        public async Task UpdateMessageAsync_Should_Throw_If_Not_Found()
        {
            var context = GetInMemoryDbContext();
            var repo = new MessageRepository(context);
            var nonExistent = CreateSampleMessage(999);

            await Assert.ThrowsAsync<Exception>(() => repo.UpdateMessageAsync(nonExistent));
        }
    }
