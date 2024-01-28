using System.Net.Mime;

using AuctionApp.Application.ApiResponses;
using AuctionApp.Application.Extensions;
using AuctionApp.Application.Features.Auction.CreateAuction;
using AuctionApp.Application.Features.Auction.GetAllAuctions;
using AuctionApp.Application.Features.Auction.GetSingleAuction;
using AuctionApp.Application.Features.Auctions.DeleteAuction;
using AuctionApp.Application.Features.Auctions.UpdateAuction;

using AuctionApp.Domain.Constants;

using AuctionApp.Infrastructure;

using MediatR;

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace AuctionApp.Host.Controllers
{
    public class AuctionsController(IMediator mediator) : BaseController
    {
        [Authorize(Roles = Roles.ADMIN)]
        [HttpPost]
        [AllowAnonymous]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<CreateAuctionResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAuction([FromBody] CreateAuctionRequest request)
        {
            var result = await mediator.Send(request);

            return result.Match(
                response => CreatedAtAction(nameof(GetSingleAuction),
                    routeValues: new { id = response.AuctionId },
                    result.ToSuccessfulApiResponse()),
                ReturnErrorResponse);
        }

        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<GetAuctionResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSingleAuction(string id)
        {
            var result = await mediator.Send(new GetSingleAuctionRequest { AuctionId = id });

            // If successful, return the event data in an ApiResponse.
            // If an error occurs, return an error response using the ReturnErrorResponse method.
            return result.Match(
                _ => Ok(result.ToSuccessfulApiResponse()),
                ReturnErrorResponse);
        }

        [Authorize(Roles = Roles.ADMIN)]
        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAuction(string id, [FromBody] UpdateAuctionRequestDto request)
        {
            var result = await mediator.Send(new UpdateAuctionRequest
            {
                AuctionId = id,
                Name = request.Name,
                StartingPriceInNaira = request.StartingPriceInNaira,
                StartingTime = request.StartingTime,
                ClosingTime = request.ClosingTime
            });
            return result.Match(_ => NoContent(), ReturnErrorResponse);
        }

        [Authorize(Roles = Roles.ADMIN)]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAuction(string id)
        {
            var result = await mediator.Send(new DeleteAuctionRequest { AuctionId = id });
            return result.Match(_ => NoContent(), ReturnErrorResponse);
        }

        [Authorize]
        [HttpGet("all")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<GetAuctionResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAuctions([FromQuery] GetAllAuctionsRequest request)
        {
            var result = await mediator.Send(request);

            // If successful, return the event data in an ApiResponse.
            // If an error occurs, return an error response using the ReturnErrorResponse method.
            return result.Match(
                _ => Ok(result.ToSuccessfulApiResponse()),
                ReturnErrorResponse);
        }
    }
}