namespace eVote360Pro.Core.Application.DTOs.PoliticalParty
{
    public class UpdatePoliticalPartyDto : BaseDto<int>
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string Acronym { get; set; }
        public string? Logo { get; set; }
    }
}
