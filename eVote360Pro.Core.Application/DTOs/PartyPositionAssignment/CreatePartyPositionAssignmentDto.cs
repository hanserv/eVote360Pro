namespace eVote360Pro.Core.Application.DTOs.PartyPositionAssignment
{
    public class CreatePartyPositionAssignmentDto
    {
        public required int CandidateId { get; set; } // Propio o de alianza
        public required int ElectivePositionId { get; set; }
    }
}
