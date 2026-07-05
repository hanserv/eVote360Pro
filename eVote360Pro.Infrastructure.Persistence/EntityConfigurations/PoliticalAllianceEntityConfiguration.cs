using eVote360Pro.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360Pro.Infrastructure.Persistence.EntityConfigurations
{
    public class PoliticalAllianceEntityConfiguration : IEntityTypeConfiguration<PoliticalAlliance>
    {
        public void Configure(EntityTypeBuilder<PoliticalAlliance> builder)
        {
            #region Basic configuration
            builder.ToTable("PoliticalAlliances");
            builder.HasKey(pa => pa.Id);
            #endregion

            #region Property configurations
            builder.Property(pa => pa.RequestDate).IsRequired();
            builder.Property(pa => pa.Status).IsRequired();
            builder.Property(pa => pa.AcceptedDate).IsRequired(false);
            #endregion

            #region Relationships
            #endregion
        }
    }
}
