namespace eVote360Pro.Core.Application.ViewModels.Candidate
{
    public class ChangeStatusCandidateViewModel
    {
        public required int Id { get; set; }
        public string? Name { get; set; }
        public bool NewStatus { get; set; }
    }
}
