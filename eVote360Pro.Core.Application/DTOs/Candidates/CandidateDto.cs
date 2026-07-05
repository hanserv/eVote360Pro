namespace eVote360Pro.Core.Application.DTOs.Candidates
{
    public class CandidateDto : BaseDto<int>
    {
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Photo { get; set; }
        public string? ElectivePositionName { get; set; }
        public required bool IsActive { get; set; }
    }
}
