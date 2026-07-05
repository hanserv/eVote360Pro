using eVote360Pro.Core.Domain.Common;

namespace eVote360Pro.Core.Domain.Entities
{
    public class ElectionParticipation : BaseEntity<int>
    {
        public required int ElectionId { get; set; }
        public required int CitizenId { get; set; }
        public required DateTime FinalizedDate { get; set; }

        public Election? Election { get; set; }
        public Citizen? Citizen { get; set; }
    }
}
