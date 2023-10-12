using Microsoft.AspNetCore.Mvc;
using Challenge.Domain.LogService;
using Challenge.Common;
using Challenge.Domain.TokenService;
using System.Security.Claims;

namespace Challenge.Services.IntegrationService.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class TokenController : Controller
    {

        #region Declarations
                
        private readonly ITokenService _tokenService;
        private readonly ILogService _logService;

        #endregion

        #region Constructor

        public TokenController( ITokenService tokenService, ILogService logService)
        {            
            _tokenService = tokenService;
            _logService = logService;
        }

        #endregion

        #region Methods        
        [HttpGet]
        public ActionResult GenerateToken(string user, string pass)
        {
            try
            {
                return Ok(ResponseFactory<string>.RequestOK(_tokenService.GenerateToken(user, pass)));
            }
            catch (Exception ex)
            {
                _logService.Add(LogKey.Error, $"Class: TokenController, Method: GenerateToken, Trace: {ex.StackTrace}, Error Message: {ex.Message}");
                return Ok(ResponseFactory<string>.RequestError(ex.Message));
            }
            finally
            {
                _logService.Generate();
            }
        }

        [HttpGet]
        public ActionResult ValidateToken(string token)
        {
            try
            {
                return Ok(ResponseFactory<ClaimsPrincipal>.RequestOK(_tokenService.GetTokenInfo(token)));
                
            }
            catch (Exception ex)
            {
                _logService.Add(LogKey.Error, $"Class: TokenController, Method: ValidateToken, Trace: {ex.StackTrace}, Error Message: {ex.Message}");
                return Ok(ResponseFactory<string>.RequestError().Message);
            }
            finally
            {
                _logService.Generate();
            }
        }        

        #endregion
    }

}