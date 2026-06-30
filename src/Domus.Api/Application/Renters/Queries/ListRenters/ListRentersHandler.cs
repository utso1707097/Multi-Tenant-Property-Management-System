using Domus.Application.Common.Interfaces;
using Domus.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Domus.Application.Renters.Queries.ListRenters;

public sealed class ListRentersHandler(IAppDbContext db)
{
    public async Task<PagedResult<RenterListItem>> HandleAsync(
        ListRentersQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var renters = db.Renters
            .AsNoTracking()
            .Include(x => x.User);

        var totalCount = await renters.CountAsync(cancellationToken);

        var hasNextPage = (page * pageSize) < totalCount;

        var items = await renters
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new RenterListItem(
                x.Id,
                x.User.Email!,
                x.User.FullName,
                x.UnitId,
                x.MoveInDate,
                x.LeaseEndDate,
                x.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<RenterListItem>(
            items,
            page,
            pageSize,
            totalCount,
            hasNextPage);
    }
}