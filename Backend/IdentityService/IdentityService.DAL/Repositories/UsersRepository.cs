using System.Linq.Expressions;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Data;

namespace IdentityService.DAL.Repositories;

public class UsersRepository(ApplicationDbContext context) : IUsersRepository
{
    public async Task<User?> GetByIdAsync(Guid id, bool withTracking = true, CancellationToken cancellationToken = default,
        params Expression<Func<User, object>>[]? includesProperties)
    {
        var query = withTracking ? 
            context.Users.AsQueryable() :
            context.Users.AsQueryable().AsNoTracking();

        if (includesProperties != null)
            foreach (var includeProperty in includesProperties)
                query = query.Include(includeProperty);

        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<User?> FirstOrDefaultAsync(Expression<Func<User, bool>> filter, CancellationToken cancellationToken = default,
        params Expression<Func<User, object>>[]? includesProperties)
    {
        var query = context.Users.AsQueryable().AsNoTracking();

        if (includesProperties != null)
            foreach (var includeProperty in includesProperties)
                query = query.Include(includeProperty);

        return await query.FirstOrDefaultAsync(filter, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> PaginatedListAllAsync(int offset, int limit, CancellationToken cancellationToken = default, 
        params Expression<Func<User, object>>[]? includesProperties)
    {
        var query = context.Users.AsQueryable().AsNoTracking();
        
        if (includesProperties != null)
            foreach (var includeProperty in includesProperties)
                query = query.Include(includeProperty);
        
        return await query
            .OrderBy(x => x.RegisteredAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> PaginatedListAsync(Expression<Func<User, bool>>? filter, int offset, int limit,
        CancellationToken cancellationToken = default, params Expression<Func<User, object>>[]? includesProperties)
    {
        var query = context.Users.AsQueryable().AsNoTracking();

        if (filter != null) query = query.Where(filter);

        if (includesProperties != null)
            foreach (var includeProperty in includesProperties)
                query = query.Include(includeProperty);

        return await query
            .OrderBy(x => x.RegisteredAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
    {
        context.Users.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(User entity, CancellationToken cancellationToken = default)
    {
        context.Users.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(Expression<Func<User, bool>>? filter, CancellationToken cancellationToken = default)
    {
        var query = context.Users.AsQueryable();

        if (filter != null) query = query.Where(filter);

        return await query.CountAsync(cancellationToken);
    }
}