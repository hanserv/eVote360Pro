using eVote360Pro.Core.Domain.Common;

namespace eVote360Pro.Core.Domain.Entities
{
    public class PartyPositionAssignment : BaseEntity<int>
    {
        public required int AssigningPartyId { get; set; } // Partido que asigna
        public required int CandidateId { get; set; } // Propio o de alianza
        public required int ElectivePositionId { get; set; }

        public PoliticalParty? AssigningParty { get; set; }
        public Candidate? Candidate { get; set; }
        public ElectivePosition? ElectivePosition { get; set; }
    }
}
