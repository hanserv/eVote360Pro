using eVote360Pro.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360Pro.Infrastructure.Persistence.EntityConfigurations
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            #region Basic configuration
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);
            #endregion

            #region Property configurations
            builder.Property(u => u.Name).IsRequired();
            builder.Property(u => u.LastName).IsRequired();
            builder.Property(u => u.Email).IsRequired().HasMaxLength(150);
            builder.Property(u => u.Username).IsRequired();
            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.Role).IsRequired();
            builder.Property(u => u.IsActive).IsRequired().HasDefaultValue(true);

            builder.HasIndex(u => u.Username).IsUnique();
            builder.HasIndex(u => u.Email).IsUnique();
            #endregion

            #region Relationships
            builder.HasOne(u => u.PoliticalParty)
                   .WithOne(p => p.User)
                   .HasForeignKey<User>(u => u.PoliticalPartyId)
                   .OnDelete(DeleteBehavior.SetNull);
            #endregion
        }
    }
}
