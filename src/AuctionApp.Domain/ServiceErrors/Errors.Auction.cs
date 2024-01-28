using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionApp.Domain.ServiceErrors
{
    public static partial class Errors
    {
        public static class Auction
        {
            public static Error CannotDeleteInProgressAuction => Error.Conflict(
                code: "Auction.CannotDeleteInProgressAuction",
                description: "Cannot delete an auction that is in progress.");
        }
    }
}