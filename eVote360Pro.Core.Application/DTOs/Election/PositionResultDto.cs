namespace eVote360Pro.Core.Application.DTOs.Election
{
    public class PositionResultDto
    {
        public required int PositionId { get; set; }
        public required string PositionName { get; set; }
        public required int TotalVotes { get; set; }
        public bool HasTie { get; set; }
        public List<CandidateResultDto> Candidates { get; set; } = [];
    }
}
