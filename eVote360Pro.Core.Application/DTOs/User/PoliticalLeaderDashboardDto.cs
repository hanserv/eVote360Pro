namespace eVote360Pro.Core.Application.DTOs.User
{
    public class PoliticalLeaderDashboardDto
    {
        public required string PoliticalPartyName { get; set; }
        public required string PoliticalPartyAcronym { get; set; }
        public required string PoliticalPartyLogo { get; set; }

        public required int TotalActiveCandidates { get; set; }
        public required int TotalInactiveCandidates { get; set; }
        public required int TotalPoliticalAlliances { get; set; }
        public required int TotalPendingAllianceRequests { get; set; }
        public required int TotalAssignedCandidates { get; set; }
    }
}
