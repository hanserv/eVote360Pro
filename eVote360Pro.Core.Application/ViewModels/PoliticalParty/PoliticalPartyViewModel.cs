namespace eVote360Pro.Core.Application.ViewModels.PoliticalParty
{
    public class PoliticalPartyViewModel : BaseViewModel<int>
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string Acronym { get; set; }
        public required string Logo { get; set; }
        public required bool IsActive { get; set; }
    }
}
