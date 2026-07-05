namespace eVote360Pro.Core.Application.ViewModels.PartyPositionAssignment
{
    public class CreatePartyPositionAssignmentViewModel
    {
        public required int CandidateId { get; set; } // Propio o de alianza
        public required int ElectivePositionId { get; set; }
    }
}
