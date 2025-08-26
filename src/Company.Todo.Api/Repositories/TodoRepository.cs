using Company.Todo.Api.Data;
using Company.Todo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Company.Todo.Api.Repositories;

public class TodoRepository(AppDbContext ctx) : GenericRepository<TodoItem>(ctx), ITodoRepository
{
    public async Task<IReadOnlyList<TodoItem>> ListPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default)
    {
        var query = _dbSet.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t => t.Title.Contains(search));

        return await query
            .OrderByDescending(t => t.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? search, CancellationToken ct = default)
    {
        var query = _dbSet.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t => t.Title.Contains(search));
        return await query.CountAsync(ct);
    }
}
