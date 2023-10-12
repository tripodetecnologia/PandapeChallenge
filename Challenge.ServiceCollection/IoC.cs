using Microsoft.Extensions.DependencyInjection;
using Challenge.DataAccess.Core;
using Challenge.DataAccess.EF;
using Challenge.Domain.LogService;
using Challenge.Domain.TokenService;
using Challenge.Domain.CandidatesService;
using Challenge.Domain.CandidateExperiencesService;

namespace Challenge.ServiceCollection
{
    public class IoC
    {
        public static void AddDependency(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ILogService, LogService>();            
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICandidatesService, CandidatesService>();
            services.AddScoped<ICandidateExperiencesService, CandidateExperiencesService>();
        }
    }
}