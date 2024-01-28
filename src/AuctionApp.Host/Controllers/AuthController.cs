using System.Net.Mime;

using AuctionApp.Application.ApiResponses;
using AuctionApp.Application.Extensions;
using AuctionApp.Application.Features.Auth;
using AuctionApp.Application.Features.Auth.Login;
using AuctionApp.Application.Features.Auth.Register;
using AuctionApp.Infrastructure;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionApp.Host.Controllers;

public class AuthController(IMediator mediator) : BaseController
{
    [HttpPost("register")]
    [AllowAnonymous]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<UserAuthResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
    {
        var result = await mediator.Send(request);
        return result.Match(_ => Ok(result.ToSuccessfulApiResponse()), ReturnErrorResponse);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<UserAuthResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        var result = await mediator.Send(request);
        return result.Match(_ => Ok(result.ToSuccessfulApiResponse()), ReturnErrorResponse);
    }
}