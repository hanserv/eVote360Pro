namespace eVote360Pro.Core.Application.DTOs.PartyPositionAssignment
{
    public class AssignmentSummaryDto : BaseDto<int>
    {
        public required string CandidateName { get; set; }
        public required string CandidateLastName { get; set; }
        public required string OriginPartyName { get; set; }
        public required string ElectivePositionName { get; set; }
        public required string CandidacyType { get; set; }
    }
}
