namespace eVote360Pro.Core.Application.ViewModels.Voting
{
    public class CandidateOptionViewModel
    {
        public required int AssignmentId { get; set; }
        public required string CandidatePhoto { get; set; }
        public required string CandidateName { get; set; }
        public required string PoliticalPartyName { get; set; }
        public required string PoliticalPartyLogo { get; set; }
    }
}
