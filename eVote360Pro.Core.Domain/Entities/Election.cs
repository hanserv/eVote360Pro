using eVote360Pro.Core.Domain.Common;
using eVote360Pro.Core.Domain.Common.Enums;

namespace eVote360Pro.Core.Domain.Entities
{
    public class Election : BaseEntity<int>
    {
        public required string Name { get; set; }
        public required DateTime Date { get; set; }
        public required ElectionStatus Status { get; set; }

        public int? TotalParties { get; set; }
        public int? TotalPositions { get; set; }
        public int? TotalCandidates { get; set; }

        public ICollection<Vote>? Votes { get; set; }
        public ICollection<VerificationCode>? VerificationCodes { get; set; }
        public ICollection<ElectionParticipation>? Participations { get; set; }
    }
}
