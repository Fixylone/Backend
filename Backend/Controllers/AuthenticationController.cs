using Backend.Application.Dtos.Requests;
using Backend.Application.Dtos.Responses;
using Backend.Application.Features.Authentication.Commands.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthenticationController(IMediator _mediator) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(GetDetailsResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequestDto registerUserRequestDto)
        {
            var result = await _mediator.Send(new RegisterUserCommand(registerUserRequestDto));
            return Ok(result);
        }
    }
}
