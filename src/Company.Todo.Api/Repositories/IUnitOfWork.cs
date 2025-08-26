namespace Company.Todo.Api.Repositories;

public interface IUnitOfWork : IAsyncDisposable
{
    ITodoRepository Todos { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
