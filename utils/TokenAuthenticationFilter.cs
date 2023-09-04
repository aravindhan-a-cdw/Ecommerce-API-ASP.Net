using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EcommerceAPI.utils
{
	public class TokenAuthenticationFilter: Attribute, IAuthorizationFilter
	{
		public TokenAuthenticationFilter()
		{
		}

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var result = true;

        }
    }
}

