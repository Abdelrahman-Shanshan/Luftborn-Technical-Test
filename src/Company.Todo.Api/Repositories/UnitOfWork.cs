using Company.Todo.Api.Data;

namespace Company.Todo.Api.Repositories;

public class UnitOfWork(AppDbContext context, ITodoRepository todos) : IUnitOfWork
{
    private readonly AppDbContext _context = context;
    public ITodoRepository Todos { get; } = todos;

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);

    public ValueTask DisposeAsync() => _context.DisposeAsync();
}
