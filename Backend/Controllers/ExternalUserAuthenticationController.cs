using System.Security.Claims;
using Backend.Application.Dtos.Responses;
using Backend.Application.Features.Authentication.Queries.LoginWithGoogleToken;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    /// <summary>
    /// The controller for handling external user related request.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ExternalUserAuthenticationController(IMediator _mediator) : ControllerBase
    {
        /// <summary>
        /// Authenticates the user with Google account. Must be called from web browser!
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task LoginWithGoogle()
        {
            await HttpContext.ChallengeAsync("oidc-google", new AuthenticationProperties
            {
                RedirectUri = "/ExternalUserAuthentication/loginWithGoogleToken"
            });
        }

        /// <summary>
        /// Returns new authentication token if user is successfully authenticated with Google.
        /// </summary>
        /// <returns>The authentication token.</returns>
        [Authorize(AuthenticationSchemes = "oidc-google")]
        [HttpGet]
        [ProducesResponseType(typeof(AuthenticateUserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthenticateUserResponseDto>> LoginWithGoogleToken()
        {
            if (User.Identity is not { IsAuthenticated: true }) return BadRequest();

            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            return Ok(await _mediator.Send(new LoginWithGoogleTokenQuery("Google", nameIdentifier, email)));
        }
    }
}