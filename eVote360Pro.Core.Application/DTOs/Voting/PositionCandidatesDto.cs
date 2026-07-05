namespace eVote360Pro.Core.Application.DTOs.Voting
{
    public class PositionCandidatesDto
    {
        public required int PositionId { get; set; }
        public required string PositionName { get; set; }
        public int? SelectedAssignmentId { get; set; }
        public List<CandidateOptionDto> Candidates { get; set; } = [];
    }
}
