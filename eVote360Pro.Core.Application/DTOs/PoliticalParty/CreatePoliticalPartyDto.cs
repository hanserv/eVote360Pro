namespace eVote360Pro.Core.Application.DTOs.PoliticalParty
{
    public class CreatePoliticalPartyDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string Acronym { get; set; }
    }
}
