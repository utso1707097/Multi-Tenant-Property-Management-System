using Domus.Api.Extensions;
using Domus.Api.Filters;
using Domus.Application.Common.Models;
using Domus.Application.Common.Results;
using Domus.Application.Units.Commands.CreateUnit;
using Domus.Application.Units.Queries.ListUnits;

namespace Domus.Api.Endpoints;

public sealed class UnitEndpoints : IEndpointGroup
{
    public void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/units")
            .WithTags("Units")
            .RequireAuthorization("OwnerOnly");

        group.MapPost("/", CreateUnit)
            .WithName("CreateUnit")
            .AddEndpointFilter<ValidationFilter<CreateUnitCommand>>()
            .Produces<CreateUnitResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapGet("/", ListUnits)
            .WithName("ListUnits")
            .Produces<PagedResult<UnitListItem>>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> CreateUnit(
        CreateUnitCommand command,
        CreateUnitHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? TypedResults.Created($"/api/v1/units/{result.Value!.Id}", result.Value)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> ListUnits(
        [AsParameters] ListUnitsQuery query,
        ListUnitsHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(query, cancellationToken);

        return TypedResults.Ok(result);
    }
}