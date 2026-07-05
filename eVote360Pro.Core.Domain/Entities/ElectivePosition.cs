using eVote360Pro.Core.Domain.Common;

namespace eVote360Pro.Core.Domain.Entities
{
    public class ElectivePosition : BaseEntity<int>
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required bool IsActive { get; set; }

        public ICollection<PartyPositionAssignment>? CandidateAssignments { get; set; }
        public ICollection<Vote>? Votes { get; set; }
    }
}
