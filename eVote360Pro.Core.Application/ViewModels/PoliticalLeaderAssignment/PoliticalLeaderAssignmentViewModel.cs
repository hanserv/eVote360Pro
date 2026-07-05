namespace eVote360Pro.Core.Application.ViewModels.PoliticalLeaderAssignment
{
    public class PoliticalLeaderAssignmentViewModel : BaseViewModel<int>
    {
        public required int UserId { get; set; }
        public required string UserFullName { get; set; }
        public required string Username { get; set; }
        public required bool IsUserActive { get; set; }

        public required int PoliticalPartyId { get; set; }
        public required string PoliticalPartyName { get; set; }
        public required string PoliticalPartyAcronym { get; set; }
        public required bool IsPoliticalPartyActive { get; set; }
    }
}
