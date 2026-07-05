using eVote360Pro.Core.Domain.Common;

namespace eVote360Pro.Core.Domain.Entities
{
    public class Candidate : BaseEntity<int>
    {
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Photo { get; set; }
        public required bool IsActive { get; set; }

        public required int PoliticalPartyId { get; set; }
        public PoliticalParty? PoliticalParty { get; set; }

        public ICollection<PartyPositionAssignment>? PositionAssignments { get; set; }
        public ICollection<Vote>? Votes { get; set; }

    }
}
