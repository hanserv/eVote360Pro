using eVote360Pro.Core.Domain.Common;

namespace eVote360Pro.Core.Domain.Entities
{
    public class PoliticalParty : BaseEntity<int>
    {
        public required string Name { get; set; }
        public required string Acronym { get; set; }
        public string? Description { get; set; }
        public required string Logo { get; set; }
        public required bool IsActive { get; set; }

        public User? User { get; set; } // Leader

        public ICollection<Candidate>? Candidates { get; set; }
        public ICollection<PartyPositionAssignment>? AssignedPositions { get; set; } // Puestos en el partido

        public ICollection<PoliticalAlliance>? AlliancesAsRequester { get; set; }
        public ICollection<PoliticalAlliance>? AlliancesAsReceiver { get; set; }
        public ICollection<Vote>? Votes { get; set; }
    }
}
