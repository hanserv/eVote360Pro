namespace eVote360Pro.Core.Application.ViewModels.Election
{
    public class PositionResultViewModel
    {
        public required int PositionId { get; set; }
        public required string PositionName { get; set; }
        public required int TotalVotes { get; set; }
        public bool HasTie { get; set; }
        public List<CandidateResultViewModel> Candidates { get; set; } = [];
    }
}
