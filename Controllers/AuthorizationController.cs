using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.Security.Claims;
using WeCareForAll.Models.Domain;
using WeCareForAll.Models.DTO;
using WeCareForAll.Repositories.Abstract;
using System.IdentityModel.Tokens.Jwt;

namespace WeCareForAll.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly DatabaseContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ITokenService tokenService;

     public AuthorizationController(DatabaseContext context, UserManager<ApplicationUser> userManager,
          RoleManager<IdentityRole> roleManager, ITokenService tokenService)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.tokenService= tokenService;

        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]LoginModel model)
        {
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {

                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

                };

                foreach(var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                var token = tokenService.GetToken(authClaims);
                var refereshtoken = tokenService.GetRefereshtoken();
                var tokenInfo = context.TokenInfos.FirstOrDefault(a => a.UserName == user.UserName);
                if(tokenInfo == null)
                {
                    var info = new TokenInfo
                    {
                        UserName = user.UserName,
                        RefereshToken = refereshtoken,
                        RefereshTokenExpiry = DateTime.UtcNow.AddDays(1)
                    };
                    context.TokenInfos.Add(info);

                }
                else
                {
                    tokenInfo.RefereshToken = refereshtoken;
                    tokenInfo.RefereshTokenExpiry = DateTime.Now.AddDays(1);
                }
                context.SaveChanges();
                return Ok(new LoginResponse
                {
                    Name = user.Name,
                    Username = user.UserName,
                    RefreshToken = refereshtoken,
                    Expiration = token.ValidTo,
                    StatusCode = 1,
                    Message = "Logged in"
                });



            }

            //logging fail condition

            return Ok(
                new LoginResponse
                {
                    StatusCode = 0,
                    Message = "Invalid UserName or password",
                    Token = "",
                    Expiration = null


                });
        }


        // after registering admin we will comment this code, because i want only one admin in this application
        [HttpPost("Registrationadmin")]
        public async Task<IActionResult> RegistrationAdmin([FromBody] RegistrationModel model)
        {
            var status = new Status();
            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "Please pass all the required fields";
                return Ok(status);
            }
            // check if user exists
            var userExists = await userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
            {
                status.StatusCode = 0;
                status.Message = "Invalid username";
                return Ok(status);
            }
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = model.Email,
                Name = model.Name
            };
            // create a user here
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "Admin Creation Failed";
                return Ok(status);
            }

            // add roles here
            // for admin registration UserRoles.Admin instead of UserRoles.Roles
            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            }

            if (await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            status.StatusCode = 1;
            status.Message = "Sucessfully registered";
            return Ok(status);
        }

         [HttpPost("Registration")]
        public async Task<IActionResult> Registration([FromBody]RegistrationModel model)
        {
            var status = new Status();

            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "Please pass all the required fields";
                return Ok(status);
            }
                // check user exist
                var userexist =  await userManager.FindByNameAsync(model.UserName);
                if (userexist != null)
                {
                    status.StatusCode = 0;
                    status.Message = "Invalid UserName";
                    return Ok(status);
                }

                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Email = model.Email,
                    Name = model.Name

                };

                //CREATED A USER HERE 
                var result = await userManager.CreateAsync(user, model.Password);
                if(!result.Succeeded) 
                {
                    status.StatusCode = 0;
                    status.Message = "User creation failed";
                    return Ok(status);

                }
            //add roles 
            if (!await roleManager.RoleExistsAsync(UserRoles.User))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            }
                if(await roleManager .RoleExistsAsync(UserRoles.User))
                {
                    await userManager.AddToRoleAsync(user, UserRoles.User);
                }


                status.StatusCode = 1;
                status.Message = "Successfully registered ";
                return Ok(status);


            }
            
        }
    }


