using Backend.Application.Dtos.Requests;
using Backend.Application.Enums;
using Backend.Application.Features.User.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController(IMediator _mediator) : ControllerBase
    {
        /// <summary>
        /// Updates the specified user.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <param name="updateUserRequestDto">The request data.</param>
        /// <returns>HTTP response indicating if this request was successful or not.</returns>
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequestDto updateUserRequestDto)
        {
            var result = await _mediator.Send(
                new UpdateUserCommand(updateUserRequestDto));

            return Ok(result);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UserAccountVerification(string email, EmailConfirmationEnum emailConfirmation)
        {
            _ = await _mediator.Send(new UserAccountVerificationCommand(email, emailConfirmation));

            return Ok();
        }
    }
}