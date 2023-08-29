using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EcommerceAPI.utils
{
    public class SecurityFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            bool hasAllowAnonymous = context.MethodInfo.GetCustomAttributes(true)
                                 .Any(em => em.GetType() == typeof(AllowAnonymousAttribute));

            var requiredPolicies = context.MethodInfo
                .GetCustomAttributes(true)
                .Concat(context.MethodInfo.DeclaringType.GetCustomAttributes(true))
                .OfType<AuthorizeAttribute>()
                .Select(attr => attr.Policy)
                .Distinct();
            //var requiredScopes = requiredPolicies.Select(p => _authorizationOptions.GetPolicy(p))
            //    .SelectMany(r => r.Requirements.OfType<ClaimsAuthorizationRequirement>())
            //    .Where(cr => cr.ClaimType == "scope")
            //    .SelectMany(r => r.AllowedValues)
            //    .Distinct()
            //    .ToList();

            if (!hasAllowAnonymous && requiredPolicies.Any())
            {
                var securityRequirement = new OpenApiSecurityRequirement()
            {
                {
                    // Put here you own security scheme, this one is an example
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            };
                operation.Security = new List<OpenApiSecurityRequirement> { securityRequirement };
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            }
        }
    }
}

