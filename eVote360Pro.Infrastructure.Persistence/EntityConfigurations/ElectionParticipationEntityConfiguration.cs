using eVote360Pro.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360Pro.Infrastructure.Persistence.EntityConfigurations
{
    public class ElectionParticipationEntityConfiguration : IEntityTypeConfiguration<ElectionParticipation>
    {
        public void Configure(EntityTypeBuilder<ElectionParticipation> builder)
        {
            #region Basic configuration
            builder.ToTable("ElectionParticipations");
            builder.HasKey(ep => ep.Id);
            #endregion

            #region Property configurations
            builder.Property(ep => ep.ElectionId).IsRequired();
            builder.Property(ep => ep.CitizenId).IsRequired();
            builder.Property(ep => ep.FinalizedDate).IsRequired();

            builder.HasIndex(ep => new { ep.ElectionId, ep.CitizenId }).IsUnique(); // Una vez por election
            #endregion

            #region Relationships
            #endregion
        }
    }

}
