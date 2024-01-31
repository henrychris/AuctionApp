using AuctionApp.Application.ApiResponses;
using AuctionApp.Application.Extensions;
using AuctionApp.Application.Features.Users.GetUser;
using AuctionApp.Infrastructure;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionApp.Host.Controllers;

public class UserController(IMediator mediator) : BaseController
{
    [Authorize]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<GetUserResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSingleUser(string id)
    {
        var result = await mediator.Send(new GetSingleUserRequest { UserId = id });

        // If successful, return the event data in an ApiResponse.
        // If an error occurs, return an error response using the ReturnErrorResponse method.
        return result.Match(
            _ => Ok(result.ToSuccessfulApiResponse()),
            ReturnErrorResponse);
    }
}