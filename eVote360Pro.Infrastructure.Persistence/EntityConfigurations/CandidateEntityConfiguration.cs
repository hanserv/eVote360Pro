using eVote360Pro.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360Pro.Infrastructure.Persistence.EntityConfigurations
{
    public class CandidateEntityConfiguration : IEntityTypeConfiguration<Candidate>
    {
        public void Configure(EntityTypeBuilder<Candidate> builder)
        {
            #region Basic configuration
            builder.ToTable("Candidates");
            builder.HasKey(c => c.Id);
            #endregion

            #region Property configurations
            builder.Property(c => c.Name).IsRequired();
            builder.Property(c => c.LastName).IsRequired();
            builder.Property(c => c.Photo).IsRequired();
            builder.Property(c => c.IsActive).IsRequired().HasDefaultValue(true);
            #endregion

            #region Relationships
            builder.HasMany(c => c.PositionAssignments)
                   .WithOne(pa => pa.Candidate)
                   .HasForeignKey(pa => pa.CandidateId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Votes)
                   .WithOne(v => v.Candidate)
                   .HasForeignKey(v => v.CandidateId)
                   .OnDelete(DeleteBehavior.Restrict);
            #endregion
        }
    }
}
