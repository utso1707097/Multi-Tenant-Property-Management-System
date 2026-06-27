using FluentValidation;

namespace Domus.Api.Filters;

public sealed class ValidationFilter<TRequest>(IValidator<TRequest> validator) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TRequest>().FirstOrDefault();
        if (request is null)
            return TypedResults.BadRequest();

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        return await next(context);
    }
}