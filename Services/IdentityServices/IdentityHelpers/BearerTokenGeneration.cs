using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Entities;
using Core.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Services.IdentityServices.IdentityHelpers
{
	public class BearerTokenManager
	{
	    public static async Task<string> GenerateToken(User user, IList<string> roleNames, RoleManager<AppRole> roleManager, string tokenKey, string issuer)
	    {
	        var claims = new List<Claim>(new[]
            {
	            new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
	            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
	        });
	        foreach (var roleName in roleNames)
	        {
	            // Find IdentityRole by name
	            var role = await roleManager.FindByNameAsync(roleName);
	            if (role != null)
	            {
	                // Convert Identity to claim and add 
	                var roleClaim = new Claim(ClaimTypes.Role, role.Name, ClaimValueTypes.String, issuer);
	                claims.Add(roleClaim);

	                // Add claims belonging to the role
	                 var roleClaims = await roleManager.GetClaimsAsync(role);
	                claims.AddRange(roleClaims);
	            }
	        }

	        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
	        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

	        var token = new JwtSecurityToken(issuer,
	            issuer,
	            claims,
	            expires:  DateTime.UtcNow.AddDays(365),
	            signingCredentials: creds);

	        return new JwtSecurityTokenHandler().WriteToken(token);
	    }

	    public static string GetCurrentUserToken(string authorizationTicket)
        {
            if (string.IsNullOrEmpty(authorizationTicket))
            {
                throw new AuthenticationException();
            }
            var chanks = authorizationTicket.Split(' ');
            return chanks.Last();
        }

    }
}
