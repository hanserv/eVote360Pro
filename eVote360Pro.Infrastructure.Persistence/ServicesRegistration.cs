using eVote360Pro.Core.Domain.Interfaces;
using eVote360Pro.Infrastructure.Persistence.Contexts;
using eVote360Pro.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eVote360Pro.Infrastructure.Persistence
{
    public static class ServicesRegistration
    {
        public static void AddPersistenceLayer(this IServiceCollection services, IConfiguration configuration)
        {
            #region Contexts
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<eVote360ProContext>(opt =>
                                              opt.UseInMemoryDatabase("AppDb"));
            }
            else
            {
                services.AddDbContext<eVote360ProContext>(opt =>
                    opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), 
                        m => m.MigrationsAssembly(typeof(eVote360ProContext).Assembly.FullName)));
            }
            #endregion

            #region Repositories
            services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
            services.AddScoped<IElectivePositionRepository,ElectivePositionRepository>();
            services.AddScoped<IElectionRepository,ElectionRepository>();
            services.AddScoped<IUserRepository,UserRepository>();
            services.AddScoped<ICitizenRepository,CitizenRepository>();
            services.AddScoped<IPoliticalPartyRepository,PoliticalPartyRepository>();
            services.AddScoped<ICandidateRepository,CandidateRepository>();
            services.AddScoped<IPoliticalAllianceRepository,PoliticalAllianceRepository>();
            services.AddScoped<IPartyPositionAssignmentRepository,PartyPositionAssignmentRepository>();
            services.AddScoped<IElectionParticipationRepository,ElectionParticipationRepository>();
            services.AddScoped<IVerificationCodeRepository,VerificationCodeRepository>();
            services.AddScoped<IVoteRepository,VoteRepository>();
            #endregion
        }
    }
}
