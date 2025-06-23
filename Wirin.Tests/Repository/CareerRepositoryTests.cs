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

namespace Wirin.Tests.Repository;

public class CareerRepositoryTests
{
    private WirinDbContext GetDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<WirinDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new WirinDbContext(options);
    }

    private CareerEntity CreateCareerEntity(int id, string name = "Test Career", string codDepartamento = "Test CodDepartamento") =>
        new CareerEntity
        {
            Id = id,
            Name = name,
            CodDepartamento = codDepartamento
        };

    [Fact]
    public async Task GetAllAsync_ReturnsAllCareers()
    {
        var dbName = nameof(GetAllAsync_ReturnsAllCareers);
        using var context = GetDbContext(dbName);
        var repo = new CareerRepository(context);

        context.Careers.AddRange(
            CreateCareerEntity(1, "Career 1"),
            CreateCareerEntity(2, "Career 2")
        );
        await context.SaveChangesAsync();

        var careers = (await repo.GetAllAsync()).ToList();

        Assert.Equal(2, careers.Count);
        Assert.Contains(careers, c => c.Name == "Career 1");
        Assert.Contains(careers, c => c.Name == "Career 2");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectCareer()
    {
        var dbName = nameof(GetByIdAsync_ReturnsCorrectCareer);
        using var context = GetDbContext(dbName);
        var repo = new CareerRepository(context);
        var entity = CreateCareerEntity(1);
        context.Careers.Add(entity);
        await context.SaveChangesAsync();

        var career = await repo.GetByIdAsync(1);

        Assert.NotNull(career);
        Assert.Equal(1, career.Id);
        Assert.Equal("Test Career", career.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithSubjects_ReturnsCareerWithSubjects()
    {
        var dbName = nameof(GetByIdAsync_WithSubjects_ReturnsCareerWithSubjects);
        using var context = GetDbContext(dbName);
        var repo = new CareerRepository(context);

        var careerEntity = CreateCareerEntity(1, "Career With Subjects");
        careerEntity.Subjects = new List<SubjectEntity>
        {
            new SubjectEntity { Id = 101, Name = "Subject 1", CareerId = 1 },
            new SubjectEntity { Id = 102, Name = "Subject 2", CareerId = 1 }
        };
        context.Careers.Add(careerEntity);
        await context.SaveChangesAsync();

        var career = await repo.GetByIdAsync(1);

        Assert.NotNull(career);
        Assert.Equal("Career With Subjects", career.Name);
        Assert.NotNull(career.Subjects);
        Assert.Equal(2, career.Subjects.Count);
        Assert.Contains(career.Subjects, s => s.Name == "Subject 1");
    }


    [Fact]
    public async Task AddAsync_AddsCareerToDatabase()
    {
        var dbName = nameof(AddAsync_AddsCareerToDatabase);
        using var context = GetDbContext(dbName);
        var repo = new CareerRepository(context);
        var careerModel = new Career { Name = "New Career", CodDepartamento = "New CodDep" };

        await repo.AddAsync(careerModel);

        var savedCareer = await context.Careers.FirstOrDefaultAsync(c => c.Name == "New Career");
        Assert.NotNull(savedCareer);
        Assert.Equal("New Career", savedCareer.Name);
        Assert.Equal("New CodDep", savedCareer.CodDepartamento);
        Assert.True(savedCareer.Id > 0); // Check if ID was generated
    }

    [Fact]
    public async Task UpdateAsync_UpdatesCareerInDatabase()
    {
        var dbName = nameof(UpdateAsync_UpdatesCareerInDatabase);
        using var context = GetDbContext(dbName);
        var repo = new CareerRepository(context);
        var entity = CreateCareerEntity(1, "Old Name");
        context.Careers.Add(entity);
        await context.SaveChangesAsync();
        context.Entry(entity).State = EntityState.Detached; // Detach to simulate a new context for update

        var careerToUpdate = new Career { Id = 1, Name = "Updated Name", CodDepartamento = "Updated CodDep" };
        await repo.UpdateAsync(careerToUpdate);

        var updatedCareer = await context.Careers.FindAsync(1);
        Assert.NotNull(updatedCareer);
        Assert.Equal("Updated Name", updatedCareer.Name);
        Assert.Equal("Updated CodDep", updatedCareer.CodDepartamento);
    }

    [Fact]
    public async Task DeleteAsync_RemovesCareerFromDatabase()
    {
        var dbName = nameof(DeleteAsync_RemovesCareerFromDatabase);
        using var context = GetDbContext(dbName);
        var repo = new CareerRepository(context);
        var entity = CreateCareerEntity(1);
        context.Careers.Add(entity);
        await context.SaveChangesAsync();

        await repo.DeleteAsync(1);

        var deletedCareer = await context.Careers.FindAsync(1);
        Assert.Null(deletedCareer);
    }
}