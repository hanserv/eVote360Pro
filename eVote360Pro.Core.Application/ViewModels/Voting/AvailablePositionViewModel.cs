namespace eVote360Pro.Core.Application.ViewModels.Voting
{
    public class AvailablePositionViewModel
    {
        public required int PositionId { get; set; }
        public required string PositionName { get; set; }
        public required int TotalParticipatingParties { get; set; }
        public required int TotalRealCandidates { get; set; }
        public required bool HasSelectedCandidate { get; set; }
    }
}
