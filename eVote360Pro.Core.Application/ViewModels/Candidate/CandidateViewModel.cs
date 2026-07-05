namespace eVote360Pro.Core.Application.ViewModels.Candidate
{
    public class CandidateViewModel : BaseViewModel<int>
    {
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Photo { get; set; }
        public required string ElectivePositionName { get; set; }
        public required bool IsActive { get; set; }
    }
}
