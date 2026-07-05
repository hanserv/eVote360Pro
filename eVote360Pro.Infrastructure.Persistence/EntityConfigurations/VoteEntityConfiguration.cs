using eVote360Pro.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360Pro.Infrastructure.Persistence.EntityConfigurations
{
    public class VoteEntityConfiguration : IEntityTypeConfiguration<Vote>
    {
        public void Configure(EntityTypeBuilder<Vote> builder)
        {
            #region Basic configuration
            builder.ToTable("Votes");
            builder.HasKey(v => v.Id);
            #endregion

            #region Property configurations
            #endregion

            #region Relationships
            #endregion
        }
    }
}
