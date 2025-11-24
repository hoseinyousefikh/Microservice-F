public class Result
{
    protected internal Result(bool isSuccess, string error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public string Error { get; }
    public bool IsFailure => !IsSuccess;

    public static Result Success() => new Result(true, string.Empty);

    public static Result Failure(string error) => new Result(false, error);
}

public class Result<T> : Result
{
    protected internal Result(T value, bool isSuccess, string error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    public T Value { get; }

    public static Result<T> Success(T value) => new Result<T>(value, true, string.Empty);

    public new static Result<T> Failure(string error) => new Result<T>(default!, false, error);
}

public class PagedResult<T> : Result<T>
{
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    protected internal PagedResult(
        T value,
        int pageNumber,
        int pageSize,
        int totalCount,
        bool isSuccess,
        string error)
        : base(value, isSuccess, error)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    public static PagedResult<T> Success(
        T value,
        int pageNumber,
        int pageSize,
        int totalCount)
        => new PagedResult<T>(value, pageNumber, pageSize, totalCount, true, string.Empty);

    public new static PagedResult<T> Failure(string error)
        => new PagedResult<T>(default!, 0, 0, 0, false, error);
}