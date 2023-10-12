using Challenge.Common;
using Challenge.Domain.LogService;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Challenge.Domain.TokenService
{
    public class TokenService : ITokenService
    {
        #region PrivateFields

        private readonly Configuration _configuration;
        private readonly ILogService _logService;
        private const string KeySigning = "DSF65FDSFSD63SDF43S5ADF354354XCZ35ASDAF5HF534354ASDFSDCVDSG545AA";
        private const string KeyEncrypt = "RTYHTJYTJF45SDF65AEEWE2DS3FWEY9Q";
        private string CurrentUser = string.Empty;
        
        #endregion

        #region Constructor

        public TokenService(ILogService logService, Configuration configuration)
        {
            _logService = logService;
            _configuration = configuration;            
        }

        public string GenerateToken(string user, string pass, double minutes = 60)
        {
            if (string.IsNullOrWhiteSpace(user))
            {
                throw new ArgumentNullException(user);
            }

            if (string.IsNullOrWhiteSpace(pass))
            {
                throw new ArgumentNullException(pass);
            }

            SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KeySigning));
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KeyEncrypt));

            SigningCredentials signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);
            EncryptingCredentials ep = new EncryptingCredentials(securityKey, SecurityAlgorithms.Aes256KW, SecurityAlgorithms.Aes256CbcHmacSha512);

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new List<Claim>() { new Claim("Data", user), new Claim("UserSession", CurrentUser) }, "Custom");


            if (user.ToLower() == _configuration.GetAppSettingValue(Constants.UserTokenKey) && pass == _configuration.GetAppSettingValue(Constants.PassTokenKey))
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "ALPHA"));
            }
            else
            {
                throw new ArgumentException(Messages.NotAuthenticated);
            }

            DateTime validityDate = DateTime.UtcNow;

            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Expires = validityDate.AddMinutes(minutes),
                Subject = claimsIdentity,
                SigningCredentials = signingCredentials,
                EncryptingCredentials = ep
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken plainToken = tokenHandler.CreateToken(securityTokenDescriptor);

            return tokenHandler.WriteToken(plainToken);
        }

        public ClaimsPrincipal GetTokenInfo(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    throw new ArgumentNullException(token);
                }

                SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KeySigning));
                SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KeyEncrypt));

                TokenValidationParameters tokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = signingKey,
                    TokenDecryptionKey = securityKey
                };

                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken validatedToken;
                ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);

                if (validatedToken.ValidTo >= DateTime.UtcNow)
                {
                    return claimsPrincipal;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logService.Add(LogKey.Error, ex.Message);
                return null;
            }
            finally
            {
                _logService.Generate();
            }
        }

        public string GenerateTokenIncludeUser(string currentUserSession, string user, string pass, double minutes = 60)
        {
            CurrentUser = currentUserSession ?? string.Empty;
            return GenerateToken(user, pass, minutes);
        }

        #endregion
    }
}
