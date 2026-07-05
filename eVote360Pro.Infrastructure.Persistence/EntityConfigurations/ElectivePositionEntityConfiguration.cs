using eVote360Pro.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360Pro.Infrastructure.Persistence.EntityConfigurations
{
    public class ElectivePositionEntityConfiguration : IEntityTypeConfiguration<ElectivePosition>
    {
        public void Configure(EntityTypeBuilder<ElectivePosition> builder)
        {
            #region Basic configuration
            builder.ToTable("ElectivePositions");
            builder.HasKey(ep => ep.Id);
            #endregion

            #region Property configurations
            builder.Property(ep => ep.Name).IsRequired();
            builder.Property(ep => ep.Description);
            builder.Property(ep => ep.IsActive).IsRequired().HasDefaultValue(true);
            #endregion

            #region Relationships
            builder.HasMany(ep => ep.CandidateAssignments)
                   .WithOne(pa => pa.ElectivePosition)
                   .HasForeignKey(pa => pa.ElectivePositionId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(ep => ep.Votes)
                   .WithOne(v => v.ElectivePosition)
                   .HasForeignKey(v => v.ElectivePositionId)
                   .OnDelete(DeleteBehavior.Restrict);
            #endregion
        }
    }
}
