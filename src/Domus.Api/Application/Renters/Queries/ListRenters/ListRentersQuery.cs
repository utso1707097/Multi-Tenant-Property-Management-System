namespace Domus.Application.Renters.Queries.ListRenters;

public sealed record ListRentersQuery(
    int Page = 1,
    int PageSize = 20);