using System.Reflection;
using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Application.Seeds;
using eVote360Pro.Core.Application.Services;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace eVote360Pro.Core.Application
{
    public static class ServicesRegistration
    {
        public static void AddApplicationLayer(this IServiceCollection services)
        {
            #region Mapster
                var config = new TypeAdapterConfig();
                config.Scan(Assembly.GetExecutingAssembly());

                services.AddSingleton(config);
                services.AddScoped<IMapper, ServiceMapper>();
            #endregion

            #region Services
            services.AddScoped<IElectivePositionService,ElectivePositionService>();
            services.AddScoped<IUserService,UserService>();
            services.AddScoped<IElectionService,ElectionService>();
            services.AddScoped<ICitizenService,CitizenService>();
            services.AddScoped<IPoliticalPartyService,PoliticalPartyService>();
            services.AddScoped<IPoliticalLeaderAssignmentService,PoliticalLeaderAssignmentService>();
            services.AddScoped<ICandidateService,CandidateService>();
            services.AddScoped<IPoliticalAllianceService,PoliticalAllianceService>();
            services.AddScoped<IPartyPositionAssignmentService,PartyPositionAssignmentService>();
            services.AddScoped<IPoliticalLeaderDashboardService,PoliticalLeaderDashboardService>();
            services.AddScoped<IVoterIdentityService,VoterIdentityService>();
            services.AddScoped<IVotingService,VotingService>();
            #endregion
        }

        #region Seeds
        public async static Task RunSeedsAsync(this IServiceProvider service)
        {
            using (var scope = service.CreateScope())
            {
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                await DefaultAdminUser.SeedAsync(userService);
            }
        }
        #endregion
    }
}
