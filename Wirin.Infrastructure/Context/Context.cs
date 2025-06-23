namespace Wirin.Infrastructure.Context;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Wirin.Domain.Models;
using Wirin.Infrastructure.Entities;

public class WirinDbContext : IdentityDbContext<UserEntity, IdentityRole<string>, string>
{
    public WirinDbContext(DbContextOptions<WirinDbContext> options): base(options) { }

    public DbSet<UserEntity> Users { get; set; }

    public DbSet<OrderEntity> Orders { get; set; }
    public DbSet<ParagraphEntity> Paragraph { get; set; }
    public DbSet<MessageEntity> Message { get; set; }
    public DbSet<OrderDeliveryEntity> OrderDeliveries { get; set; }
    public DbSet<OrderSequenceEntity> OrderSequences { get; set; }
    public DbSet<ParagraphAnnotationEntity> ParagraphAnnotations { get; set; }
    public DbSet<OrderTrasabilityEntity> OrderTrasability { get; set; }
    public DbSet<StudentDeliveryEntity> StudentDeliveries { get; set; }
    public DbSet<CareerEntity> Careers { get; set; }
    public DbSet<SubjectEntity> Subjects { get; set; }
    public DbSet<OrderFeedbackEntity> OrderFeedbacks { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

    }

}

