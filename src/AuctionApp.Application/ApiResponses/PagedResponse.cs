using Microsoft.EntityFrameworkCore;

namespace AuctionApp.Application.ApiResponses;

public class PagedResponse<T>
{
    public IEnumerable<T> Items { get; set; } = null!;
    private int CurrentPage { get; init; }
    private int TotalPages { get; init; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public async Task<PagedResponse<T>> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResponse<T>
        {
            Items = items,
            TotalCount = count,
            PageSize = pageSize,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize)
        };
    }
}