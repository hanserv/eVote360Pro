using eVote360Pro.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360Pro.Infrastructure.Persistence.EntityConfigurations
{
    public class CitizenEntityConfiguration : IEntityTypeConfiguration<Citizen>
    {
        public void Configure(EntityTypeBuilder<Citizen> builder)
        {
            #region Basic configuration
            builder.ToTable("Citizens");
            builder.HasKey(c => c.Id);
            #endregion

            #region Property configurations
            builder.Property(c => c.DocumentId).IsRequired().HasMaxLength(20);
            builder.Property(c => c.Name).IsRequired();
            builder.Property(c => c.LastName).IsRequired();
            builder.Property(c => c.Email).IsRequired().HasMaxLength(150);
            builder.Property(c => c.IsActive).IsRequired().HasDefaultValue(true);

            builder.HasIndex(c => c.DocumentId).IsUnique();
            #endregion

            #region Relationships
            builder.HasMany(c => c.VerificationCodes)
                   .WithOne(vc => vc.Citizen)
                   .HasForeignKey(vc => vc.CitizenId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Votes)
               .WithOne(v => v.Citizen)
               .HasForeignKey(v => v.CitizenId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Participations)
                   .WithOne(ep => ep.Citizen)
                   .HasForeignKey(ep => ep.CitizenId)
                   .OnDelete(DeleteBehavior.Cascade);

            #endregion
        }
    }
}
