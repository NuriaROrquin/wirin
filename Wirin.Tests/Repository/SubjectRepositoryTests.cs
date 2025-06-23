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

public class SubjectRepositoryTests
{
    private WirinDbContext GetDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<WirinDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        var context = new WirinDbContext(options);
        // Seed a career for FK constraints if necessary for most tests
        if (!context.Careers.Any(c => c.Id == 1))
        {
            context.Careers.Add(new CareerEntity { Id = 1, Name = "Default Career", CodDepartamento = "Default" });
            context.SaveChanges();
        }
        return context;
    }

    private SubjectEntity CreateSubjectEntity(int id, string name = "Test Subject", int careerId = 1) =>
        new SubjectEntity
        {
            Id = id,
            Name = name,
            CareerId = careerId
        };

    [Fact]
    public async Task GetAllAsync_ReturnsAllSubjects()
    {
        var dbName = nameof(GetAllAsync_ReturnsAllSubjects);
        using var context = GetDbContext(dbName);
        var repo = new SubjectRepository(context);

        // ✅ Asegurarse de que exista la carrera con Id = 2
        if (!context.Careers.Any(c => c.Id == 2))
        {
            context.Careers.Add(new CareerEntity { Id = 2, Name = "Extra Career", CodDepartamento = "Extra" });
            await context.SaveChangesAsync();
        }

        context.Subjects.AddRange(
            CreateSubjectEntity(1, "Subject 1", 1),
            CreateSubjectEntity(2, "Subject 2", 2)
        );
        await context.SaveChangesAsync();

        var subjects = (await repo.GetAllAsync()).ToList();

        Assert.Equal(2, subjects.Count);
        Assert.Contains(subjects, s => s.Name == "Subject 1");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectSubject()
    {
        var dbName = nameof(GetByIdAsync_ReturnsCorrectSubject);
        using var context = GetDbContext(dbName);
        var repo = new SubjectRepository(context);
        var entity = CreateSubjectEntity(1);
        context.Subjects.Add(entity);
        await context.SaveChangesAsync();

        var subject = await repo.GetByIdAsync(1);

        Assert.NotNull(subject);
        Assert.Equal(1, subject.Id);
        Assert.Equal("Test Subject", subject.Name);
        Assert.Equal(1, subject.CareerId);
    }

    [Fact]
    public async Task GetByCareerIdAsync_ReturnsSubjectsForCareer()
    {
        var dbName = nameof(GetByCareerIdAsync_ReturnsSubjectsForCareer);
        using var context = GetDbContext(dbName);
        var repo = new SubjectRepository(context);

        // Ensure another career exists for isolation
        if (!context.Careers.Any(c => c.Id == 2))
        {
            context.Careers.Add(new CareerEntity { Id = 2, Name = "Other Career", CodDepartamento = "1" });
            await context.SaveChangesAsync();
        }

        context.Subjects.AddRange(
            CreateSubjectEntity(1, "Sub Career 1", 1),
            CreateSubjectEntity(2, "Sub Career 1 Again", 1),
            CreateSubjectEntity(3, "Sub Career 2", 2) // Belongs to other career
        );
        await context.SaveChangesAsync();

        var subjectsForCareer1 = (await repo.GetByCareerIdAsync(1)).ToList();

        Assert.Equal(2, subjectsForCareer1.Count);
        Assert.All(subjectsForCareer1, s => Assert.Equal(1, s.CareerId));
        Assert.Contains(subjectsForCareer1, s => s.Name == "Sub Career 1");
    }

    [Fact]
    public async Task AddAsync_AddsSubjectToDatabase()
    {
        var dbName = nameof(AddAsync_AddsSubjectToDatabase);
        using var context = GetDbContext(dbName);
        var repo = new SubjectRepository(context);
        var subjectModel = new Subject { Name = "New Subject", CareerId = 1 };

        await repo.AddAsync(subjectModel);

        var savedSubject = await context.Subjects.FirstOrDefaultAsync(s => s.Name == "New Subject");
        Assert.NotNull(savedSubject);
        Assert.Equal("New Subject", savedSubject.Name);
        Assert.Equal(1, savedSubject.CareerId);
        Assert.True(savedSubject.Id > 0);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesSubjectInDatabase()
    {
        var dbName = nameof(UpdateAsync_UpdatesSubjectInDatabase);
        using var context = GetDbContext(dbName);
        var repo = new SubjectRepository(context);
        var entity = CreateSubjectEntity(1, "Old Name", 1);
        context.Subjects.Add(entity);
        await context.SaveChangesAsync();
        context.Entry(entity).State = EntityState.Detached;

        var subjectToUpdate = new Subject { Id = 1, Name = "Updated Name", CareerId = 1 };
        await repo.UpdateAsync(subjectToUpdate);

        var updatedSubject = await context.Subjects.FindAsync(1);
        Assert.NotNull(updatedSubject);
        Assert.Equal("Updated Name", updatedSubject.Name);
    }

    [Fact]
    public async Task DeleteAsync_RemovesSubjectFromDatabase()
    {
        var dbName = nameof(DeleteAsync_RemovesSubjectFromDatabase);
        using var context = GetDbContext(dbName);
        var repo = new SubjectRepository(context);
        var entity = CreateSubjectEntity(1);
        context.Subjects.Add(entity);
        await context.SaveChangesAsync();

        await repo.DeleteAsync(1);

        var deletedSubject = await context.Subjects.FindAsync(1);
        Assert.Null(deletedSubject);
    }
}