namespace eVote360Pro.Core.Application.DTOs.Candidates
{
    public class UpdateCandidateDto : BaseDto<int>
    {
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public string? Photo { get; set; }
    }
}
