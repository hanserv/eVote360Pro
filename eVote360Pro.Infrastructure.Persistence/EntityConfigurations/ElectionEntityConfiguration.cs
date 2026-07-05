using eVote360Pro.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360Pro.Infrastructure.Persistence.EntityConfigurations
{
    public class ElectionEntityConfiguration : IEntityTypeConfiguration<Election>
    {
        public void Configure(EntityTypeBuilder<Election> builder)
        {
            #region Basic configuration
            builder.ToTable("Elections");
            builder.HasKey(e => e.Id);
            #endregion

            #region Property configurations
            builder.Property(e => e.Name).IsRequired();
            builder.Property(e => e.Date).IsRequired();
            builder.Property(e => e.Status).IsRequired();
            builder.Property(e => e.TotalParties).IsRequired(false);
            builder.Property(e => e.TotalPositions).IsRequired(false);
            builder.Property(e => e.TotalCandidates).IsRequired(false);
            #endregion

            #region Relationships
            builder.HasMany(e => e.Votes)
                   .WithOne(v => v.Election)
                   .HasForeignKey(v => v.ElectionId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.VerificationCodes)
                   .WithOne(vc => vc.Election)
                   .HasForeignKey(vc => vc.ElectionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Participations)
                   .WithOne(ep => ep.Election)
                   .HasForeignKey(ep => ep.ElectionId)
                   .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
