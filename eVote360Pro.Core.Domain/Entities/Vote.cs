using eVote360Pro.Core.Domain.Common;

namespace eVote360Pro.Core.Domain.Entities
{
    public class Vote : BaseEntity<int>
    {
        public required int ElectionId { get; set; }
        public required int CitizenId { get; set; }
        public required int ElectivePositionId { get; set; }
        public int? CandidateId { get; set; } // Ninguno null
        public int? PoliticalPartyId { get; set; }

        public Election? Election { get; set; }
        public Citizen? Citizen { get; set; }
        public ElectivePosition? ElectivePosition { get; set; }
        public Candidate? Candidate { get; set; }
        public PoliticalParty? PoliticalParty { get; set; }
    }
}
