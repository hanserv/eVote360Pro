namespace eVote360Pro.Core.Application.ViewModels.PoliticalParty
{
    public class ChangeStatusPoliticalPartyViewModel
    {
        public required int Id { get; set; }
        public string? Name { get; set; }
        public bool NewStatus { get; set; }
    }
}
