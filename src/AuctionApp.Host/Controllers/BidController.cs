using AuctionApp.Infrastructure;

using MediatR;

namespace AuctionApp.Host.Controllers
{
    public class BidController(IMediator mediator) : BaseController
    {
    }
}