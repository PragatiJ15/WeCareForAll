using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using WeCareForAll.Models.Domain;
using WeCareForAll.Models.DTO;
using WeCareForAll.Repositories.Abstract;

namespace WeCareForAll.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly DatabaseContext context;
        private readonly ITokenService service;
        

        public TokenController(DatabaseContext context, ITokenService service)
        {
            this.service = service;
            this.context = context;

        }
        [HttpPost]
        public IActionResult Refresh (RefereshTokenRequest tokenapimodel)
        {
                if(tokenapimodel is null)
                    
                {
                return BadRequest("Invalid client request");
                }
                string accessToken = tokenapimodel.AccessToken;
            var refreshToken = tokenapimodel.RefreshToken;
            var principal = service.GetClaimsPrincipalFromExpriedToken(accessToken);
            var username = principal.Identity.Name;
            var user = context.TokenInfos.SingleOrDefault(u=> u.UserName== username);
            if(user is null || user.RefereshToken != refreshToken || user.RefereshTokenExpiry <= DateTime.Now) 
            {
                return BadRequest("Invalid client request");
            }

            var newAccessToken = service.GetToken(principal.Claims);
            var newrefreshToken = service.GetRefereshtoken();
            user.RefereshToken = newrefreshToken;
            context.SaveChanges();
            return Ok(new RefereshTokenRequest()
            {
                AccessToken = newAccessToken.TokenString,
                RefreshToken = newrefreshToken

            });



        }

    }
}
