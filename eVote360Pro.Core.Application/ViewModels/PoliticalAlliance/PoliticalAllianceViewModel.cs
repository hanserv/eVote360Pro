using eVote360Pro.Core.Application.ViewModels.PoliticalParty;
using eVote360Pro.Core.Domain.Common.Enums;

namespace eVote360Pro.Core.Application.ViewModels.PoliticalAlliance
{
    public class PoliticalAllianceViewModel : BaseViewModel<int>
    {
        public required DateTime RequestDate { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public required AllianceStatus Status { get; set; }
        public required PoliticalPartyViewModel RequesterParty { get; set; }
        public required PoliticalPartyViewModel TargetParty { get; set; }
    }
}
