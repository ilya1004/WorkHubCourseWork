using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityService.DAL.Configurations;

public class EmployerProfileConfiguration : IEntityTypeConfiguration<EmployerProfile>
{
    public void Configure(EntityTypeBuilder<EmployerProfile> builder)
    {
        builder.ToTable("EmployerProfiles");

        builder.HasIndex(e => e.UserId)
            .IsUnique();

        builder.Property(e => e.CompanyName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(e => e.About)
            .IsRequired(false);

        builder.Property(e => e.StripeCustomerId)
            .IsRequired(false)
            .HasMaxLength(256);

        builder.HasOne(e => e.Industry)
            .WithMany(i => i.EmployerProfiles)
            .HasForeignKey(e => e.IndustryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}