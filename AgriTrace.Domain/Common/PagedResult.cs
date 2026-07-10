namespace AgriTrace.Domain.Common;


public class PagedResult<T>
{

    public IReadOnlyList<T> Items { get; private set; }


    public int TotalCount { get; private set; }


    public int PageNumber { get; private set; }


    public int PageSize { get; private set; }


    public int TotalPages =>
        (int)Math.Ceiling(
            TotalCount / (double)PageSize);



    public PagedResult(
        IReadOnlyList<T> items,
        int totalCount,
        int pageNumber,
        int pageSize)
    {

        Items = items;

        TotalCount = totalCount;

        PageNumber = pageNumber;

        PageSize = pageSize;

    }

}