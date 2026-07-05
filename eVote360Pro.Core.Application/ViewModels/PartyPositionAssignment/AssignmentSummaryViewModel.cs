namespace eVote360Pro.Core.Application.ViewModels.PartyPositionAssignment
{
    public class AssignmentSummaryViewModel : BaseViewModel<int>
    {
        public required string CandidateName { get; set; }
        public required string CandidateLastName { get; set; }
        public required string OriginPartyName { get; set; }
        public required string ElectivePositionName { get; set; }
        public required string CandidacyType { get; set; }
    }
}
