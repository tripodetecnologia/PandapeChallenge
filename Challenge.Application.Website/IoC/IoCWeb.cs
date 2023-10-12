using Challenge.Domain.LogService;
using Challenge.Domain.TokenService;

namespace Challenge.Application.Website.IoC
{
    public static class IoCWeb
    {
        public static void AddDependency(IServiceCollection services)
        {
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<ITokenService, TokenService>();                   
        }
    }
}
