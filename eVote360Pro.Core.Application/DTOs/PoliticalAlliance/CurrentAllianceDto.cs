namespace eVote360Pro.Core.Application.DTOs.PoliticalAlliance
{
    public class CurrentAllianceDto
    {
        public required int Id { get; set; }
        public required string AlliedPartyName { get; set; } 
        public required string AlliedPartyAcronym { get; set; } 
        public required DateTime AcceptedDate { get; set; }
    }
}
