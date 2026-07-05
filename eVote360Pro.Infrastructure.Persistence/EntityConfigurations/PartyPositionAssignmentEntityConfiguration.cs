using eVote360Pro.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360Pro.Infrastructure.Persistence.EntityConfigurations
{
    public class PartyPositionAssignmentEntityConfiguration : IEntityTypeConfiguration<PartyPositionAssignment>
    {
        public void Configure(EntityTypeBuilder<PartyPositionAssignment> builder)
        {
            #region Basic configuration
            builder.ToTable("PartyPositionAssignments");
            builder.HasKey(pa => pa.Id);
            #endregion

            #region Property configurations
            #endregion

            #region Relationships
            #endregion
        }
    }
}
