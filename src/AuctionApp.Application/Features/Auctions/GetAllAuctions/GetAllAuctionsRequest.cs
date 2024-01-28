using AuctionApp.Application.ApiResponses;
using AuctionApp.Application.Contracts;
using AuctionApp.Application.Features.Auction.GetSingleAuction;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AuctionApp.Application.Features.Auction.GetAllAuctions;

public class GetAllAuctionsRequest : QueryStringParameters, IRequest<ErrorOr<PagedResponse<GetAuctionResponse>>>
{
    public string? Name { get; set; }
    public string? Status { get; set; }
    public DateTime? Date { get; set; }
}

public class GetAllAuctionsRequestHandler(IAuctionService auctionService, ILogger<GetAllAuctionsRequestHandler> logger) : IRequestHandler<GetAllAuctionsRequest, ErrorOr<PagedResponse<GetAuctionResponse>>>
{
    public async Task<ErrorOr<PagedResponse<GetAuctionResponse>>> Handle(GetAllAuctionsRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching auctions...\nRequest: {request}", request);

        var query = auctionService.GetAuctionsQuery();
        query = ApplyFilters(request, query);

        var results = query.Select(x => AuctionMapper.ToGetAuctionResponse(x));
        var response = await new PagedResponse<GetAuctionResponse>().ToPagedList(results, request.PageNumber, request.PageSize);

        logger.LogInformation("Successfully fetched auctions. Returned {count} auctions.", response.TotalCount);
        return response;
    }

    private static IQueryable<Domain.Entities.Auction> ApplyFilters(GetAllAuctionsRequest request, IQueryable<Domain.Entities.Auction> query)
    {
        if (!string.IsNullOrEmpty(request.Name))
        {
            query = query.Where(x => x.Name.Contains(request.Name));
        }

        if (!string.IsNullOrEmpty(request.Status))
        {
            var status = Enum.TryParse<Domain.Enums.AuctionStatus>(request.Status, true, out var parsedStatus);
            query = query.Where(x => x.Status == parsedStatus);
        }

        if (request.Date.HasValue)
        {
            // where date is between start and end of day
            var date = request.Date.Value;
            var startOfDay = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            var endOfDay = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);

            query = query.Where(x => x.StartingTime >= startOfDay && x.StartingTime <= endOfDay);
        }

        return query;
    }

}