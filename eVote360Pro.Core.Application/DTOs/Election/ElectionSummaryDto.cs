namespace eVote360Pro.Core.Application.DTOs.Election
{
    public class ElectionSummaryDto
    {
        public required string ElectionName { get; set; } 
        public required DateTime RealizationDate { get; set; }
        public required int TotalParticipatingParties { get; set; }
        public required int TotalParticipatingCandidates { get; set; }
        public required int TotalCitizensVoted { get; set; }
    }
}
