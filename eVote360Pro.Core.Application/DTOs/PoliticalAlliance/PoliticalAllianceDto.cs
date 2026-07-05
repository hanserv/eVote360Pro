using eVote360Pro.Core.Application.DTOs.PoliticalParty;
using eVote360Pro.Core.Domain.Common.Enums;

namespace eVote360Pro.Core.Application.DTOs.PoliticalAlliance
{
    public class PoliticalAllianceDto : BaseDto<int>
    {
        public required DateTime RequestDate { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public required AllianceStatus Status { get; set; }
        public required PoliticalPartyDto RequesterParty { get; set; }
        public required PoliticalPartyDto TargetParty { get; set; }
    }
}
