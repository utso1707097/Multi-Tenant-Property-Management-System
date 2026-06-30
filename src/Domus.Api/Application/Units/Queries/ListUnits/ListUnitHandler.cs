
using Domus.Application.Common.Interfaces;
using Domus.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Domus.Application.Units.Queries.ListUnits;


public sealed class ListUnitsHandler(IAppDbContext db)
{
    public async Task<PagedResult<UnitListItem>> HandleAsync(
    ListUnitsQuery query,
    CancellationToken cancellationToken = default)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var units = db.Units.AsNoTracking();

        if (query.PropertyId is not null)
        {
            units = units.Where(x => x.PropertyId == query.PropertyId);
        }

        var totalCount = await units.CountAsync(cancellationToken);

        var items = await units
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new UnitListItem(
                x.Id,
                x.PropertyId,
                x.UnitNumber,
                x.Floor,
                x.Bedrooms,
                x.RentAmount,
                x.Currency,
                x.Status,
                x.CreatedAt))
            .ToListAsync(cancellationToken);

        var hasNextPage = (page * pageSize) < totalCount;

        return new PagedResult<UnitListItem>(
            items,
            page,
            pageSize,
            totalCount,
            hasNextPage);
    }
}