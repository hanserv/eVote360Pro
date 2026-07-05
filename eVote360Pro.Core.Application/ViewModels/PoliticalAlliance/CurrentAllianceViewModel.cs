namespace eVote360Pro.Core.Application.ViewModels.PoliticalAlliance
{
    public class CurrentAllianceViewModel
    {
        public required int Id { get; set; }
        public required string AlliedPartyName { get; set; } 
        public required string AlliedPartyAcronym { get; set; } 
        public required DateTime AcceptedDate { get; set; }
    }
}
