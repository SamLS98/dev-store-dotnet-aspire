using DevStore.Core.Messages.Integration;
using DevStore.Identity.API.Models;
using DevStore.MessageBus;
using DevStore.WebAPI.Core.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Identity.Interfaces;
using NetDevPack.Security.Jwt.Core.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DevStore.Identity.API.Controllers
{
    [Route("api/identity")]
    public class AuthController(
        IJwtBuilder jwtBuilder,
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        IMessageBus bus) : MainController
    {
        public readonly SignInManager<IdentityUser> SignInManager = signInManager;
        public readonly UserManager<IdentityUser> UserManager = userManager;

        [HttpPost("new-account")]
        public async Task<ActionResult> Register(NewUser newUser)
        {

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var user = new IdentityUser
            {
                UserName = newUser.Email,
                Email = newUser.Email,
                EmailConfirmed = true
            };

            var result = await UserManager.CreateAsync(user, newUser.Password);

            if (result.Succeeded)
            {
                var customerResult = await RegisterUser(newUser);

                if (!customerResult.ValidationResult.IsValid)
                {
                    await UserManager.DeleteAsync(user);
                    return CustomResponse(customerResult.ValidationResult);
                }

                var jwt = await jwtBuilder
                                            .WithEmail(newUser.Email)
                                            .WithJwtClaims()
                                            .WithUserClaims()
                                            .WithUserRoles()
                                            .WithRefreshToken()
                                            .BuildUserResponse();

                return CustomResponse(jwt);
            }

            foreach (var error in result.Errors)
            {
                AddErrorToStack(error.Description);
            }

            return CustomResponse();
        }

        [HttpPost("auth")]
        public async Task<ActionResult> Login(UserLogin userLogin)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var result = await SignInManager.PasswordSignInAsync(userLogin.Email, userLogin.Password,
                false, true);

            if (result.Succeeded)
            {

                var jwt = await jwtBuilder
                                            .WithEmail(userLogin.Email)
                                            .WithJwtClaims()
                                            .WithUserClaims()
                                            .WithUserRoles()
                                            .WithRefreshToken()
                                            .BuildUserResponse();
                return CustomResponse(jwt);
            }

            if (result.IsLockedOut)
            {
                AddErrorToStack("User temporary blocked. Too many tries.");
                return CustomResponse();
            }

            AddErrorToStack("User or Password incorrect");
            return CustomResponse();
        }

        private async Task<ResponseMessage> RegisterUser(NewUser newUser)
        {
            var user = await UserManager.FindByEmailAsync(newUser.Email);

            var userRegistered = new UserRegisteredIntegrationEvent(Guid.Parse(user.Id), newUser.Name, newUser.Email, newUser.SocialNumber);

            try
            {
                return await bus.RequestAsync<UserRegisteredIntegrationEvent, ResponseMessage>(userRegistered);
            }
            catch (Exception)
            {
                await UserManager.DeleteAsync(user);
                throw;
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken([FromBody] string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                AddErrorToStack("Invalid Refresh Token");
                return CustomResponse();
            }

            var token = await jwtBuilder.ValidateRefreshToken(refreshToken);

            if (!token.IsValid)
            {
                AddErrorToStack("Expired Refresh Token");
                return CustomResponse();
            }

            var jwt = await jwtBuilder
                                        .WithUserId(token.UserId)
                                        .WithJwtClaims()
                                        .WithUserClaims()
                                        .WithUserRoles()
                                        .WithRefreshToken()
                                        .BuildUserResponse();

            return CustomResponse(jwt);
        }

#if DEBUG
        [HttpPost("validate-jwt")]
        public async Task<ActionResult> ValidateJwt([FromServices] IJwtService jwtService, [FromForm] string jwt)
        {
            var handler = new JsonWebTokenHandler();

            var result = await handler.ValidateTokenAsync(jwt, new TokenValidationParameters()
            {
                ValidIssuer = "https://devstore.academy",
                ValidAudience = "DevStore",
                ValidateAudience = true,
                ValidateIssuer = true,
                RequireSignedTokens = false,
                IssuerSigningKey = await jwtService.GetCurrentSecurityKey(),
            });

            if (!result.IsValid)
                return BadRequest();

            return Ok(result.Claims.Select(s => new { s.Key, s.Value }));
        }

#endif
    }
}