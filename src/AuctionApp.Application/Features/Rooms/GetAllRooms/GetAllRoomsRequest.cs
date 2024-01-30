using AuctionApp.Application.ApiResponses;
using AuctionApp.Application.Contracts;
using AuctionApp.Application.Features.Rooms.GetSingleRoom;
using AuctionApp.Domain.Entities;
using AuctionApp.Domain.Enums;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AuctionApp.Application.Features.Rooms.GetAllRooms;

public class GetAllRoomsRequest : QueryStringParameters, IRequest<ErrorOr<PagedResponse<GetRoomResponse>>>
{
    public string? AuctionId { get; set; }
    public string? Status { get; set; }
}

public class GetAllRoomsRequestHandler(IRoomService roomService, ILogger<GetAllRoomsRequestHandler> logger)
    : IRequestHandler<GetAllRoomsRequest, ErrorOr<PagedResponse<GetRoomResponse>>>

{
    public async Task<ErrorOr<PagedResponse<GetRoomResponse>>> Handle(GetAllRoomsRequest request,
                                                                      CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching rooms...\nRequest: {request}", request);

        IQueryable<BiddingRoom> query = roomService.GetRoomsQuery();
        query = ApplyFilters(request, query);

        var results = query.Select(x => RoomMapper.ToGetRoomResponse(x));
        var response =
            await new PagedResponse<GetRoomResponse>().ToPagedList(results, request.PageNumber, request.PageSize);

        logger.LogInformation("Successfully fetched rooms. Returned {count} rooms.", response.TotalCount);
        return response;
    }

    private static IQueryable<BiddingRoom> ApplyFilters(GetAllRoomsRequest request,
                                                        IQueryable<BiddingRoom> query)
    {
        if (!string.IsNullOrEmpty(request.AuctionId))
        {
            query = query.Where(x => x.AuctionId == request.AuctionId);
        }

        if (!string.IsNullOrEmpty(request.Status))
        {
            var status = Enum.TryParse<RoomStatus>(request.Status, true, out var parsedStatus);
            query = query.Where(x => x.Status == parsedStatus);
        }

        return query;
    }
}