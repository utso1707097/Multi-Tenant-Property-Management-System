using Domus.Api.Extensions;
using Domus.Api.Filters;
using Domus.Application.Auth;
using Domus.Application.Auth.Commands.AcceptInvite;
using Domus.Application.Auth.Commands.Login;
using Domus.Application.Auth.Commands.RefreshToken;
using Domus.Application.Auth.Commands.RegisterOwner;
using Domus.Application.Auth.Commands.RevokeToken;
using Domus.Application.Common.Results;

namespace Domus.Api.Endpoints;

public sealed class AuthEndpoints : IEndpointGroup
{
    public void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth")
            .WithTags("Auth")
            .AllowAnonymous();

        group.MapPost("/register", RegisterOwner)
            .WithName("RegisterOwner")
            .AddEndpointFilter<ValidationFilter<RegisterOwnerCommand>>()
            .Produces<RegisterOwnerResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/login", Login)
            .WithName("Login")
            .AddEndpointFilter<ValidationFilter<LoginCommand>>()
            .Produces<AuthResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);

        group.MapPost("/refresh", Refresh)
            .WithName("RefreshToken")
            .AddEndpointFilter<ValidationFilter<RefreshTokenCommand>>()
            .Produces<AuthResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapPost("/revoke", Revoke)
            .WithName("RevokeToken")
            .AddEndpointFilter<ValidationFilter<RevokeTokenCommand>>()
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapPost("/accept-invite", AcceptInvite)
            .WithName("AcceptInvite")
            .AddEndpointFilter<ValidationFilter<AcceptInviteCommand>>()
            .Produces<AcceptInviteResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status410Gone);
    }

    private static async Task<IResult> RegisterOwner(
        RegisterOwnerCommand command,
        RegisterOwnerHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsSuccess
            ? TypedResults.Created($"/api/v1/auth/users/{result.Value!.Id}", result.Value)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> Login(
        LoginCommand command,
        LoginHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> Refresh(
        RefreshTokenCommand command,
        RefreshTokenHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> Revoke(
        RevokeTokenCommand command,
        RevokeTokenHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsSuccess
            ? TypedResults.Ok()
            : result.ToProblemDetails();
    }

    private static async Task<IResult> AcceptInvite(
        AcceptInviteCommand command,
        AcceptInviteHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.ToProblemDetails();
    }
}