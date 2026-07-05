namespace eVote360Pro.Core.Application.ViewModels.ElectivePosition
{
    public class ElectivePositionViewModel : BaseViewModel<int>
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required bool IsActive { get; set; }
    }
}
