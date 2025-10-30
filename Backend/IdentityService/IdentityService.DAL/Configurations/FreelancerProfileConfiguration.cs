using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityService.DAL.Configurations;

public class FreelancerProfileConfiguration : IEntityTypeConfiguration<FreelancerProfile>
{
    public void Configure(EntityTypeBuilder<FreelancerProfile> builder)
    {
        builder.ToTable("FreelancerProfiles");

        builder.HasIndex(f => f.UserId)
            .IsUnique();

        builder.Property(f => f.FirstName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(f => f.LastName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(f => f.About)
            .IsRequired(false);

        builder.Property(f => f.StripeAccountId)
            .HasMaxLength(256);
    }
}