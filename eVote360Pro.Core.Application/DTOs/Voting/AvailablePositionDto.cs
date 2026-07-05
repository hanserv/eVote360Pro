namespace eVote360Pro.Core.Application.DTOs.Voting
{
    public class AvailablePositionDto
    {
        public required int PositionId { get; set; }
        public required string PositionName { get; set; }
        public required int TotalParticipatingParties { get; set; }
        public required int TotalRealCandidates { get; set; }
        public required bool HasSelectedCandidate { get; set; }
    }
}
