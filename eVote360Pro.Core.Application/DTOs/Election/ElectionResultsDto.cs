namespace eVote360Pro.Core.Application.DTOs.Election
{
    public class ElectionResultsDto
    {
        public required int ElectionId { get; set; }
        public required string ElectionName { get; set; } = null!;
        public List<PositionResultDto> PositionResults { get; set; } = [];
    }
}
