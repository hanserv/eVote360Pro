namespace eVote360Pro.Core.Application.DTOs.Election
{
    public class CreateElectionDto
    {
        public required string Name { get; set; }
        public required DateTime Date { get; set; }
    }
}
