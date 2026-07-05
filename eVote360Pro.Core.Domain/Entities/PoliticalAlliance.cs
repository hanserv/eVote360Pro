using eVote360Pro.Core.Domain.Common;
using eVote360Pro.Core.Domain.Common.Enums;

namespace eVote360Pro.Core.Domain.Entities
{
    public class PoliticalAlliance : BaseEntity<int>
    {
        public required DateTime RequestDate { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public required AllianceStatus Status { get; set; }
        public required int RequesterPartyId { get; set; }
        public required int TargetPartyId { get; set; }

        public PoliticalParty? RequesterParty { get; set; }
        public PoliticalParty? TargetParty { get; set; }
    }
}
