using System.Reflection;
using eVote360Pro.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace eVote360Pro.Infrastructure.Persistence.Contexts
{
    public class eVote360ProContext : DbContext
    {
        public eVote360ProContext(DbContextOptions<eVote360ProContext> options) : base(options) { }

        public DbSet<Election> Elections { get; set; }
        public DbSet<ElectivePosition> ElectivePositions { get; set; }
        public DbSet<PoliticalParty> PoliticalParties { get; set; }
        public DbSet<PoliticalAlliance> PoliticalAlliances { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Citizen> Citizens { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<VerificationCode> VerificationCodes { get; set; }
        public DbSet<PartyPositionAssignment> PartyPositionAssignments { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<ElectionParticipation> ElectionParticipations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
