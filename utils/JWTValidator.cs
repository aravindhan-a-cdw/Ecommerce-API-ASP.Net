using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace EcommerceAPI.utils
{
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

        //private bool HasAccessTokenType(ClaimsPrincipal principal)
        //{
        //    return principal.Claims.FirstOrDefault(x => x.Type == AppConsts.TokenType)?.Value ==
        //           TokenType.AccessToken.To().ToString();
        //}
    }
}

