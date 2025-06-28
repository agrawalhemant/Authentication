namespace Authentication.Contracts.Pagination;

public class PageResult<T>
{
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public IEnumerable<T?> Result { get; set; }
}