using AuctionApp.Application.Contracts;
using AuctionApp.Domain.Entities.Notifications;
using AuctionApp.Infrastructure;

using Microsoft.AspNetCore.Mvc;

namespace AuctionApp.Host.Controllers;

public class MailController(IMailService mailService) : BaseController
{
    [HttpPost("")]
    public async Task<IActionResult> SendMailAsync([FromBody] MailData mailData)
    {
        var result = await mailService.SendAsync(mailData, new CancellationToken());

        if (result)
        {
            return StatusCode(StatusCodes.Status200OK, "Mail has successfully been sent.");
        }

        return StatusCode(StatusCodes.Status500InternalServerError,
            "An error occured. The Mail could not be sent.");
    }
}