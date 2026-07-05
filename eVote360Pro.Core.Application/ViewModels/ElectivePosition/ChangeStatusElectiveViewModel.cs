namespace eVote360Pro.Core.Application.ViewModels.ElectivePosition
{
    public class ChangeStatusElectiveViewModel
    {
        public required int Id { get; set; }
        public string? Name { get; set; }
        public bool NewStatus { get; set; }
    }
}
