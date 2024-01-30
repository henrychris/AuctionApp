using System.Net.Mime;

using AuctionApp.Application.ApiResponses;
using AuctionApp.Application.Extensions;
using AuctionApp.Application.Features.Bids.MakeBid;
using AuctionApp.Application.Features.Rooms.CreateRoom;
using AuctionApp.Application.Features.Rooms.EndRoomAuction;
using AuctionApp.Application.Features.Rooms.GetAllRooms;
using AuctionApp.Application.Features.Rooms.GetSingleRoom;
using AuctionApp.Application.Features.Rooms.JoinRoom;
using AuctionApp.Application.Features.Rooms.LeaveRoom;
using AuctionApp.Application.Features.Rooms.OpenRoom;
using AuctionApp.Application.Features.Rooms.StartRoomAuction;
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

    [Authorize(Roles = Roles.USER)]
    [HttpPost("{id}/join")]
    [ProducesResponseType(typeof(ApiResponse<GetRoomResponse>), StatusCodes.Status204NoContent)]
    public async Task<IActionResult> JoinRoom(string id, [FromBody] JoinRoomRequestDto requestDto)
    {
        var result = await mediator.Send(new JoinRoomRequest { RoomId = id, ConnectionId = requestDto.ConnectionId });
        return result.Match(_ => NoContent(), ReturnErrorResponse);
    }

    [Authorize(Roles = Roles.USER)]
    [HttpPost("{id}/leave")]
    [ProducesResponseType(typeof(ApiResponse<GetRoomResponse>), StatusCodes.Status204NoContent)]
    public async Task<IActionResult> LeaveRoom(string id, [FromBody] LeaveRoomRequestDto requestDto)
    {
        var result = await mediator.Send(new LeaveRoomRequest { RoomId = id, ConnectionId = requestDto.ConnectionId });
        return result.Match(_ => NoContent(),
            ReturnErrorResponse);
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpPost("{id}/start")]
    [ProducesResponseType(typeof(ApiResponse<GetRoomResponse>), StatusCodes.Status204NoContent)]
    public async Task<IActionResult> StartRoomAuction(string id)
    {
        var result = await mediator.Send(new StartRoomAuctionRequest { RoomId = id });
        return result.Match(_ => NoContent(),
            ReturnErrorResponse);
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpPost("{id}/open")]
    [ProducesResponseType(typeof(ApiResponse<GetRoomResponse>), StatusCodes.Status204NoContent)]
    public async Task<IActionResult> OpenRoomAuction(string id)
    {
        var result = await mediator.Send(new OpenRoomRequest { RoomId = id });
        return result.Match(_ => NoContent(),
            ReturnErrorResponse);
    }

    [Authorize(Roles = Roles.USER)]
    [HttpPost("{id}/bid")]
    [ProducesResponseType(typeof(ApiResponse<GetRoomResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bid(string id, [FromBody] BidRequestDto requestDto)
    {
        var result = await mediator.Send(new MakeBidRequest
        {
            RoomId = id, ConnectionId = requestDto.ConnectionId, BidAmountInNaira = requestDto.BidAmountInNaira
        });
        return result.Match(_ => Ok(result.ToSuccessfulApiResponse()),
            ReturnErrorResponse);
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpPost("{id}/end")]
    [ProducesResponseType(typeof(ApiResponse<GetRoomResponse>), StatusCodes.Status204NoContent)]
    public async Task<IActionResult> EndRoomAuction(string id)
    {
        var result = await mediator.Send(new EndRoomAuctionRequest { RoomId = id });
        return result.Match(_ => NoContent(),
            ReturnErrorResponse);
    }

    [Authorize]
    [HttpGet("all")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<GetRoomResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRooms([FromQuery] GetAllRoomsRequest request)
    {
        var result = await mediator.Send(request);

        // If successful, return the event data in an ApiResponse.
        // If an error occurs, return an error response using the ReturnErrorResponse method.
        return result.Match(
            _ => Ok(result.ToSuccessfulApiResponse()),
            ReturnErrorResponse);
    }
}