namespace Piloopinas.Application.DTOs.Common;

public record PagedResult<T>(
    List<T> Items,
    int TotalCount,
    int Page,
    int PageSize
)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}

public record ApiResponse<T>(
    bool Success,
    string? Message,
    T? Data
);

public record ApiError(
    string Message,
    string? Details = null
);
