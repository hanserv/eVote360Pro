using eVote360Pro.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360Pro.Infrastructure.Persistence.EntityConfigurations
{
    public class VerificationCodeEntityConfiguration : IEntityTypeConfiguration<VerificationCode>
    {
        public void Configure(EntityTypeBuilder<VerificationCode> builder)
        {
            #region Basic configuration
            builder.ToTable("VerificationCodes");
            builder.HasKey(vc => vc.Id);
            #endregion

            #region Property configurations
            builder.Property(vc => vc.Code).IsRequired();
            builder.Property(vc => vc.GenerationDate).IsRequired();
            builder.Property(vc => vc.ExpirationDate).IsRequired();
            builder.Property(vc => vc.IsUsed).IsRequired().HasDefaultValue(false);
            #endregion

            #region Relationships
            #endregion
        }
    }
}
