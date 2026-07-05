namespace eVote360Pro.Core.Application.ViewModels.Voting
{
    public class PositionCandidatesViewModel
    {
        public required int PositionId { get; set; }
        public required string PositionName { get; set; }
        public int? SelectedAssignmentId { get; set; }
        public List<CandidateOptionViewModel> Candidates { get; set; } = [];
    }
}
