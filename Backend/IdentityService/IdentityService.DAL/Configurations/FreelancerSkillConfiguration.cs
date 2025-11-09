using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityService.DAL.Configurations;

public class FreelancerSkillConfiguration : IEntityTypeConfiguration<CvSkill>
{
    public void Configure(EntityTypeBuilder<CvSkill> builder)
    {
        builder.ToTable("FreelancerSkills");

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(256);
    }
}