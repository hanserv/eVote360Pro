using eVote360Pro.Core.Application.DTOs.User;
using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Domain.Common.Enums;
using eVote360Pro.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace eVote360Pro.Core.Application.Services
{
    public class PoliticalLeaderDashboardService : IPoliticalLeaderDashboardService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICandidateRepository _candidateRepository;
        private readonly IPoliticalAllianceRepository _politicalAllianceRepository;
        private readonly IPartyPositionAssignmentRepository _partyPositionAssignmentRepository;

        public PoliticalLeaderDashboardService(IUserRepository userRepository, ICandidateRepository candidateRepository, IPoliticalAllianceRepository politicalAllianceRepository, IPartyPositionAssignmentRepository partyPositionAssignmentRepository)
        {
            _userRepository = userRepository;
            _candidateRepository = candidateRepository;
            _politicalAllianceRepository = politicalAllianceRepository;
            _partyPositionAssignmentRepository = partyPositionAssignmentRepository;
        }

        public async Task<Result<PoliticalLeaderDashboardDto>> GetDashboardDataAsync(int currentUserId)
        {
            var currentUser = await _userRepository.GetAllQueryInclude(["PoliticalParty"])
                        .FirstOrDefaultAsync(u => u.Id == currentUserId);

            if (currentUser is null || !currentUser.IsActive)
            {
                return Result<PoliticalLeaderDashboardDto>.Failure(error: "The authenticated user doesnt exist or is inactive.");
            }

            if (currentUser.Role != UserRole.PoliticalLeader)
            {
                return Result<PoliticalLeaderDashboardDto>.Failure(error: "You need to be a political leader.");
            }

            if (!currentUser.PoliticalPartyId.HasValue || currentUser.PoliticalParty is null)
            {
                return Result<PoliticalLeaderDashboardDto>.Failure(error: "You are not affiliated with any political party. Please contact an administrator.");
            }

            if (!currentUser.PoliticalParty.IsActive)
            {
                return Result<PoliticalLeaderDashboardDto>.Failure(error: "The political party assigned to you is inactive.");
            }

            int partyId = currentUser.PoliticalPartyId.Value;

            var activeCandidatesCount = await _candidateRepository.CountCandidatesByStatusAsync(partyId, isActive: true);

            var inactiveCandidatesCount = await _candidateRepository.CountCandidatesByStatusAsync(partyId, isActive: false);

            var alliancesCount = await _politicalAllianceRepository.CountAcceptedAlliancesAsync(partyId);

            var pendingRequestsCount = await _politicalAllianceRepository.CountPendingReceivedRequestsAsync(partyId);

            var assignmentsCount = await _partyPositionAssignmentRepository.CountAssignmentsByPartyAsync(partyId);

            var dashboardDto = new PoliticalLeaderDashboardDto
            {
                PoliticalPartyName = currentUser.PoliticalParty.Name,
                PoliticalPartyAcronym = currentUser.PoliticalParty.Acronym,
                PoliticalPartyLogo = currentUser.PoliticalParty.Logo,

                TotalActiveCandidates = activeCandidatesCount,
                TotalInactiveCandidates = inactiveCandidatesCount,
                TotalPoliticalAlliances = alliancesCount,
                TotalPendingAllianceRequests = pendingRequestsCount,
                TotalAssignedCandidates = assignmentsCount
            };

            return Result<PoliticalLeaderDashboardDto>.Success(dashboardDto);
        }
    }
}
