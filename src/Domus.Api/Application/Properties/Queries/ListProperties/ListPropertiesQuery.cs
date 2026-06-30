namespace Domus.Application.Properties.Queries.ListProperties;

public sealed record ListPropertiesQuery(
    int Page = 1,
    int PageSize = 20);