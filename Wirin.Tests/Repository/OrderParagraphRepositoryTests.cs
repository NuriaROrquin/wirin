using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wirin.Domain.Models;
using Wirin.Infrastructure.Context;
using Wirin.Infrastructure.Entities;
using Wirin.Infrastructure.Repositories;
using Xunit;

public class OrderParagraphRepositoryTests
{
    private WirinDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<WirinDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new WirinDbContext(options);
    }

    [Fact]
    public async Task GetAllParagraphsByOrderIdAsync_ReturnsAllParagraphsForOrder()
    {
        var context = GetDbContext();
        var repo = new OrderParagraphRepository(context);

        context.Paragraph.AddRange(new List<ParagraphEntity>
        {
            new ParagraphEntity { Id = 1, OrderId = 1, PageNumber = 1, ParagraphText = "Texto 1", HasError = false, Confidence = 0.9 },
            new ParagraphEntity { Id = 2, OrderId = 1, PageNumber = 2, ParagraphText = "Texto 2", HasError = true, Confidence = 0.7 },
            new ParagraphEntity { Id = 3, OrderId = 2, PageNumber = 1, ParagraphText = "Otro texto", HasError = false, Confidence = 0.8 }
        });
        await context.SaveChangesAsync();

        var paragraphs = await repo.GetAllParagraphsByOrderIdAsync(1);

        Assert.Equal(2, paragraphs.Count);
        Assert.All(paragraphs, p => Assert.Equal(1, p.OrderId));
    }

    [Fact]
    public async Task GetParagraphByIdAsync_ReturnsCorrectParagraph()
    {
        var context = GetDbContext();
        var repo = new OrderParagraphRepository(context);

        context.Paragraph.Add(new ParagraphEntity
        {
            Id = 1,
            OrderId = 1,
            PageNumber = 5,
            ParagraphText = "Texto prueba",
            HasError = false,
            Confidence = 0.95
        });
        await context.SaveChangesAsync();

        var paragraph = await repo.GetParagraphByIdAsync(1, 5);

        Assert.NotNull(paragraph);
        Assert.Equal(1, paragraph.OrderId);
        Assert.Equal(5, paragraph.PageNumber);
        Assert.Equal("Texto prueba", paragraph.ParagraphText);
    }

    [Fact]
    public async Task GetParagraphByIdAsync_ReturnsNullIfNotFound()
    {
        var context = GetDbContext();
        var repo = new OrderParagraphRepository(context);

        var paragraph = await repo.GetParagraphByIdAsync(99, 1);

        Assert.Null(paragraph);
    }

    [Fact]
    public async Task SaveAsync_AddsNewParagraph()
    {
        var context = GetDbContext();
        var repo = new OrderParagraphRepository(context);

        var paragraphToSave = new Paragraph
        {
            OrderId = 1,
            PageNumber = 1,
            ParagraphText = "Nuevo párrafo",
            HasError = false,
            Confidence = 0.88
        };

        await repo.SaveAsync(paragraphToSave);

        var paragraphInDb = await context.Paragraph.FirstOrDefaultAsync(p => p.OrderId == 1 && p.PageNumber == 1);

        Assert.NotNull(paragraphInDb);
        Assert.Equal("Nuevo párrafo", paragraphInDb.ParagraphText);
    }

    [Fact]
    public async Task UpdateParagraphAsync_UpdatesExistingParagraph()
    {
        var context = GetDbContext();
        var repo = new OrderParagraphRepository(context);

        var existing = new ParagraphEntity
        {
            Id = 1,
            OrderId = 1,
            PageNumber = 2,
            ParagraphText = "Texto original",
            HasError = false,
            Confidence = 0.75
        };
        context.Paragraph.Add(existing);
        await context.SaveChangesAsync();

        var updatedParagraph = new Paragraph
        {
            OrderId = 1,
            PageNumber = 2,
            ParagraphText = "Texto modificado",
            HasError = true,
            Confidence = 0.95
        };

        await repo.UpdateParagraphAsync(updatedParagraph);

        var paragraphInDb = await context.Paragraph.FirstOrDefaultAsync(p => p.OrderId == 1 && p.PageNumber == 2);

        Assert.NotNull(paragraphInDb);
        Assert.Equal("Texto modificado", paragraphInDb.ParagraphText);
        Assert.True(paragraphInDb.HasError);
        Assert.Equal(0.95, paragraphInDb.Confidence);
    }
}
