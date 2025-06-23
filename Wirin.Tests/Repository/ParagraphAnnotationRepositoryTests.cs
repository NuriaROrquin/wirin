using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wirin.Domain.Models;
using Wirin.Infrastructure.Context;
using Wirin.Infrastructure.Entities;
using Wirin.Infrastructure.Repositories;
using Xunit;

public class ParagraphAnnotationRepositoryTests
{
    private WirinDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<WirinDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // DB única por test
            .Options;

        return new WirinDbContext(options);
    }

    [Fact]
    public async Task SaveParagraphAnnotationAsync_SavesAnnotationCorrectly()
    {
        // Arrange
        var context = GetDbContext();
        var repository = new ParagraphAnnotationRepository(context);

        var annotation = new ParagraphAnnotation
        {
            OrderId = 1,
            ParagraphId = 2,
            AnnotationText = "Comentario de prueba",
            UserId = "usuario123"
        };

        // Act
        await repository.SaveParagraphAnnotationAsync(annotation);

        // Assert
        var saved = await context.ParagraphAnnotations.FirstOrDefaultAsync();
        Assert.NotNull(saved);
        Assert.Equal(1, saved.OrderId);
        Assert.Equal(2, saved.ParagraphId);
        Assert.Equal("Comentario de prueba", saved.AnnotationText);
        Assert.Equal("usuario123", saved.UserId);
        Assert.True((DateTime.UtcNow - saved.CreationDate).TotalSeconds < 5);
    }

    [Fact]
    public async Task GetAllParagraphAnnotationsByParagraphIdAsync_ReturnsCorrectAnnotations()
    {
        // Arrange
        var context = GetDbContext();
        context.ParagraphAnnotations.AddRange(new List<ParagraphAnnotationEntity>
        {
            new ParagraphAnnotationEntity { OrderId = 1, ParagraphId = 2, AnnotationText = "Anotación A", UserId = "user1", CreationDate = DateTime.UtcNow },
            new ParagraphAnnotationEntity { OrderId = 1, ParagraphId = 2, AnnotationText = "Anotación B", UserId = "user2", CreationDate = DateTime.UtcNow },
            new ParagraphAnnotationEntity { OrderId = 1, ParagraphId = 3, AnnotationText = "Otra Anotación", UserId = "user3", CreationDate = DateTime.UtcNow }
        });
        await context.SaveChangesAsync();

        var repository = new ParagraphAnnotationRepository(context);

        // Act
        var result = await repository.GetAllParagraphAnnotationsByParagraphIdAsync(1, 2);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, r => Assert.Equal(2, r.ParagraphId));
    }
}
