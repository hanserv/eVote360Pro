namespace eVote360Pro.Core.Application.DTOs.PoliticalLeaderAssignment
{
    public class CreatePoliticalLeaderAssignmentDto
    {
        public required int UserId { get; set; }
        public required int PoliticalPartyId { get; set; }
    }
}
