using eVote360Pro.Core.Domain.Common;

namespace eVote360Pro.Core.Domain.Entities
{
    public class VerificationCode : BaseEntity<int>
    {
        public required string Code { get; set; }
        public required DateTime GenerationDate { get; set; }
        public required DateTime ExpirationDate { get; set; }
        public required bool IsUsed { get; set; }
        public required int CitizenId { get; set; }
        public required int ElectionId { get; set; }

        public Citizen? Citizen { get; set; }
        public Election? Election { get; set; }
    }
}
