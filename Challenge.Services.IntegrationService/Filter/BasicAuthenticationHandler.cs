using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Challenge.Domain.TokenService;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Challenge.Services.IntegrationService.Filter
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {

        private readonly ITokenService _tokenService;

        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                                          ILoggerFactory logger,
                                          UrlEncoder encoder,
                                          ISystemClock clock,
                                          ITokenService tokenService) : base(options, logger, encoder, clock)
        {
            _tokenService = tokenService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {

            var endpoint = Context.GetEndpoint();

            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                return AuthenticateResult.NoResult();
            }

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing Authorization Header");
            }

            if (string.IsNullOrWhiteSpace(Request.Headers["Authorization"]))
            {
                Response.StatusCode = 401;
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            var token = await Task.Run(() => _tokenService.GetTokenInfo(Request.Headers["Authorization"]!));

            if (token == null)
            {
                Response.StatusCode = 401;
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            var identity = new ClaimsIdentity(token.Claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
