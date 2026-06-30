using Domus.Application.Common.Interfaces;
using Domus.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Domus.Application.Renters.Queries.ListRenters;

public sealed class ListRentersHandler(IAppDbContext db, TimeProvider clock)
{
    public async Task<PagedResult<RenterListItem>> HandleAsync(
        ListRentersQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var now = clock.GetUtcNow();

        var renters = db.Renters.AsNoTracking();

        var totalCount = await renters.CountAsync(cancellationToken);

        var hasNextPage = (page * pageSize) < totalCount;

        var items = await renters
            .Join(
                db.Users.AsNoTracking(),
                r => r.UserId,
                u => u.Id,
                (r, u) => new { Renter = r, User = u })
            .OrderByDescending(x => x.Renter.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new RenterListItem(
                x.Renter.Id,
                x.User.Email!,
                x.User.FullName,
                x.Renter.UnitId,
                x.Renter.MoveInDate,
                x.Renter.LeaseEndDate,
                x.Renter.InviteToken != null && x.Renter.InviteExpires > now,
                x.Renter.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<RenterListItem>(
            items,
            page,
            pageSize,
            totalCount,
            hasNextPage);
    }
}