using Domus.Api.Extensions;
using Domus.Api.Filters;
using Domus.Application.Common.Models;
using Domus.Application.Common.Results;
using Domus.Application.Properties.Commands.CreateProperty;
using Domus.Application.Properties.Queries.ListProperties;

namespace Domus.Api.Endpoints;

public sealed class PropertyEndpoints : IEndpointGroup
{
    public void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/properties")
            .WithTags("Properties")
            .RequireAuthorization("OwnerOnly");

        group.MapPost("/", CreateProperty)
            .WithName("CreateProperty")
            .AddEndpointFilter<ValidationFilter<CreatePropertyCommand>>()
            .Produces<CreatePropertyResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapGet("/", ListProperties)
            .WithName("ListProperties")
            .Produces<PagedResult<PropertyListItem>>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> CreateProperty(
        CreatePropertyCommand command,
        CreatePropertyHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? TypedResults.Created($"/api/v1/properties/{result.Value!.Id}", result.Value)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> ListProperties(
        [AsParameters] ListPropertiesQuery query,
        ListPropertiesHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(query, cancellationToken);

        return TypedResults.Ok(result);
    }
}
