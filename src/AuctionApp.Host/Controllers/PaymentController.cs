using AuctionApp.Application.Features.Payments.CompletePayment;
using AuctionApp.Domain.Constants;
using AuctionApp.Infrastructure;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionApp.Host.Controllers;

public class PaymentController(IMediator mediator) : BaseController
{
    
    [HttpPost("complete/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CompletePayment(string id)
    {
        var result = await mediator.Send(new CompletePaymentCommand
        {
           InvoiceId = id
        });
        return result.Match(_ => Ok(),
            ReturnErrorResponse);
    }

}