namespace eVote360Pro.Core.Application.DTOs.Candidates
{
    public class CreateCandidateDto
    {
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Photo { get; set; }
    }
}
