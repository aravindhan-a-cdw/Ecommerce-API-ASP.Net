using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace EcommerceAPI.utils
{
    /*
     * @author Aravindhan A
     * @description This is a class for custom validation of jwt token
     */
    public class JWTValidator: ISecurityTokenValidator
	{
        private readonly SecurityTokenHandler _securityTokenHandler;
        private readonly IConnectionMultiplexer _redis;

        public JWTValidator(IConnectionMultiplexer redis)
		{
            _securityTokenHandler = new JwtSecurityTokenHandler();
            _redis = redis;
		}

        public bool CanValidateToken => true;

        public int MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;

        public bool CanReadToken(string securityToken)
        {
            return _securityTokenHandler.CanReadToken(securityToken);
        }


        /*
         * @author Aravindhan A
         * @description This method validates token and ensures only the token in Redis is not allowed to authenticate
         */
        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            var principal = _securityTokenHandler.ValidateToken(securityToken, validationParameters, out validatedToken);
            
            var db = _redis.GetDatabase();

            var value = db.StringGet(securityToken);
            if (value != RedisValue.Null)
            {
                throw new SecurityTokenException("Invalid Token! Token logged out.");
            }
            return principal;
        }
    }
}

