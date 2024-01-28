using System.Net.Mime;

using AuctionApp.Application.ApiResponses;
using AuctionApp.Application.Extensions;
using AuctionApp.Application.Features.Rooms;
using AuctionApp.Application.Features.Rooms.CreateRoom;
using AuctionApp.Application.Features.Rooms.GetSingleRoom;
using AuctionApp.Domain.Constants;
using AuctionApp.Infrastructure;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionApp.Host.Controllers;

public class RoomsController(IMediator mediator) : BaseController
{
    [Authorize(Roles = Roles.ADMIN)]
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<CreateRoomResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest request)
    {
        var result = await mediator.Send(request);

        return result.Match(
            response => CreatedAtAction(nameof(GetSingleRoom),
                routeValues: new { id = response.RoomId },
                result.ToSuccessfulApiResponse()),
            ReturnErrorResponse);
    }

    [Authorize]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<GetRoomResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSingleRoom(string id)
    {
        var result = await mediator.Send(new GetSingleRoomRequest { RoomId = id });

        // If successful, return the event data in an ApiResponse.
        // If an error occurs, return an error response using the ReturnErrorResponse method.
        return result.Match(
            _ => Ok(result.ToSuccessfulApiResponse()),
            ReturnErrorResponse);
    }
}