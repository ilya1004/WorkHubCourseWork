using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectsService.Infrastructure.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(p => p.Description)
            .IsRequired(false);

        builder.Property(p => p.EmployerUserId)
            .IsRequired();

        builder.Property(p => p.Budget)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.PaymentIntentId)
            .HasMaxLength(256);

        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.FreelancerApplications)
            .WithOne(fa => fa.Project)
            .HasForeignKey(fa => fa.ProjectId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Lifecycle)
            .WithOne(l => l.Project)
            .HasForeignKey<Lifecycle>(l => l.ProjectId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
