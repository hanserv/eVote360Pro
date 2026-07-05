using eVote360Pro.Core.Domain.Common;

namespace eVote360Pro.Core.Domain.Entities
{
    public class Citizen : BaseEntity<int>
    {
        public required string DocumentId { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required bool IsActive { get; set; }

        public ICollection<Vote>? Votes { get; set; }
        public ICollection<VerificationCode>? VerificationCodes { get; set; }
        public ICollection<ElectionParticipation>? Participations { get; set; }
    }
}
