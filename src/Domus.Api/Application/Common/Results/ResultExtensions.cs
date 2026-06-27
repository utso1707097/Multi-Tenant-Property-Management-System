namespace Domus.Application.Common.Results;

public static class ResultExtensions
{
    public static IResult ToProblemDetails(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Cannot map a successful result to ProblemDetails.");

        var error = result.Error!;
        return TypedResults.Problem(
            title: error.Message,
            statusCode: error.StatusCode,
            extensions: new Dictionary<string, object?> { ["code"] = error.Code });
    }
}