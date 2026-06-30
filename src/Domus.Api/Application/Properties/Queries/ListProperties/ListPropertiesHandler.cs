
using Domus.Application.Common.Interfaces;
using Domus.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Domus.Application.Properties.Queries.ListProperties;

public sealed class ListPropertiesHandler(IAppDbContext db)
{
    public async Task<PagedResult<PropertyListItem>> HandleAsync(
        ListPropertiesQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var properties = db.Properties.AsNoTracking();

        var totalCount = await properties.CountAsync(cancellationToken);

        var hasNextPage = (page * pageSize) < totalCount;

        var items = await properties
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new PropertyListItem(
                x.Id,
                x.Name,
                x.AddressLine,
                x.City,
                x.Country,
                x.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<PropertyListItem>(
            items,
            page,
            pageSize,
            totalCount,
            hasNextPage);
    }
}