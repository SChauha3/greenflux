using SmartCharging.Api.Services;

namespace SmartCharging.Api.Extensions
{
    public static class ResultExtensions
    {
        public static IResult ToApiResult<T>(this Result<T> result, string uri)
        {
            if (result.IsSuccess)
                return Results.Created($"{uri}/{result.Value}", new { id = result.Value });

            return Results.Problem(
            title: "Resource creation failed",
            detail: result.Error,
            statusCode: result.ErrorType switch
            {
                ErrorType.NotFound => 404,
                ErrorType.InValidCapacity => 400,
                ErrorType.UniqueConnector => 409,
                ErrorType.MinimumOneConnector => 400,
                _ => 500
            });
        }

        public static IResult ToApiResult(this Result result)
        {
            if (result.IsSuccess)
                return Results.NoContent();

            return Results.Problem(
            title: "Resource creation failed",
            detail: result.Error,
            statusCode: result.ErrorType switch
            {
                ErrorType.NotFound => 404,
                ErrorType.InValidCapacity => 400,
                ErrorType.UniqueConnector => 409,
                ErrorType.MinimumOneConnector => 400,
                _ => 500
            });
        }
    }
}
