using System.Net.Mime;

using AuctionApp.Application.ApiResponses;
using AuctionApp.Application.Extensions;
using AuctionApp.Application.Features.Bids.MakeBid;
using AuctionApp.Domain.Constants;
using AuctionApp.Infrastructure;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionApp.Host.Controllers
{
    public class BidController(IMediator mediator) : BaseController
    {
        [Authorize(Roles = Roles.USER)]
        [HttpPost("new")]
        [AllowAnonymous]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<MakeBidResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> MakeBid([FromBody] MakeBidRequest request)
        {
            var result = await mediator.Send(request);

            return result.Match(
                _ => Ok(result.ToSuccessfulApiResponse()),
                ReturnErrorResponse);
        }
    }
}