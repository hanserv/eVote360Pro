namespace eVote360Pro.Core.Application.DTOs.Election
{
    public class CandidateResultDto
    {
        public required string CandidateName { get; set; } 
        public required string PartyName { get; set; } 
        public int VoteCount { get; set; }
        public double Percentage { get; set; }
        public bool IsWinner { get; set; }
        public bool IsNone { get; set; }
    }
}
