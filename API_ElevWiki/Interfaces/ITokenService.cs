using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace API_ElevWiki.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateAccessToken(IEnumerable<Claim> claims);
        Task<string> GenerateRandomToken();

        Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token);

        Task<List<Claim>> AddRoleClaim(string email);
        Task<List<Claim>> AddEmailClaim(string email);

        Task<string> FetchEmailFromClaims(string token);
    }
}
