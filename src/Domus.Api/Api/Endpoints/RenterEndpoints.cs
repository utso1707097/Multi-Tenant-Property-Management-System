using Domus.Api.Extensions;
using Domus.Api.Filters;
using Domus.Application.Common.Models;
using Domus.Application.Common.Results;
using Domus.Application.Renters.Commands.InviteRenter;
using Domus.Application.Renters.Queries.ListRenters;

namespace Domus.Api.Endpoints;

public sealed class RenterEndpoints : IEndpointGroup
{
    public void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/renters")
            .WithTags("Renters")
            .RequireAuthorization("OwnerOnly");

        group.MapPost("/", InviteRenter)
            .WithName("InviteRenter")
            .AddEndpointFilter<ValidationFilter<InviteRenterCommand>>()
            .Produces<InviteRenterResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapGet("/", ListRenters)
            .WithName("ListRenters")
            .Produces<PagedResult<RenterListItem>>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> InviteRenter(
        InviteRenterCommand command,
        InviteRenterHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? TypedResults.Created($"/api/v1/renters/{result.Value!.RenterId}", result.Value)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> ListRenters(
        [AsParameters] ListRentersQuery query,
        ListRentersHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(query, cancellationToken);

        return TypedResults.Ok(result);
    }
}