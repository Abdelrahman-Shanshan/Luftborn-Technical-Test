using Company.Todo.Api.Models;

namespace Company.Todo.Api.Repositories;

public interface ITodoRepository : IGenericRepository<TodoItem>
{
    Task<IReadOnlyList<TodoItem>> ListPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default);
    Task<int> CountAsync(string? search, CancellationToken ct = default);
}
