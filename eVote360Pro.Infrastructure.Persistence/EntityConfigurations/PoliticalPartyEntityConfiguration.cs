using eVote360Pro.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360Pro.Infrastructure.Persistence.EntityConfigurations
{
    public class PoliticalPartyEntityConfiguration : IEntityTypeConfiguration<PoliticalParty>
    {
        public void Configure(EntityTypeBuilder<PoliticalParty> builder)
        {
            #region Basic configuration
            builder.ToTable("PoliticalParties");
            builder.HasKey(p => p.Id);
            #endregion

            #region Property configurations
            builder.Property(p => p.Name).IsRequired();
            builder.Property(p => p.Acronym).IsRequired().HasMaxLength(20);
            builder.Property(p => p.Description);
            builder.Property(p => p.Logo).IsRequired();
            builder.Property(p => p.IsActive).IsRequired().HasDefaultValue(true);

            builder.HasIndex(p => p.Acronym).IsUnique();
            #endregion

            #region Relationships
            builder.HasMany(p => p.Candidates)
                   .WithOne(c => c.PoliticalParty)
                   .HasForeignKey(c => c.PoliticalPartyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.AssignedPositions)
                   .WithOne(pa => pa.AssigningParty)
                   .HasForeignKey(pa => pa.AssigningPartyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.AlliancesAsRequester)
                   .WithOne(a => a.RequesterParty)
                   .HasForeignKey(a => a.RequesterPartyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.AlliancesAsReceiver)
                   .WithOne(a => a.TargetParty)
                   .HasForeignKey(a => a.TargetPartyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Votes)
                   .WithOne(v => v.PoliticalParty)
                   .HasForeignKey(v => v.PoliticalPartyId)
                   .OnDelete(DeleteBehavior.Restrict);
            #endregion
        }
    }
}
