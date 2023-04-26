using System.Security.Claims;
using WeCareForAll.Models.DTO;

namespace WeCareForAll.Repositories.Abstract
{
    public interface ITokenService
    {
        TokenResponse GetToken(IEnumerable<Claim> claims);
        string GetRefereshtoken();
        ClaimsPrincipal GetClaimsPrincipalFromExpriedToken(string token);

    }
}
