using Company.Todo.Api.Dtos;

namespace Company.Todo.Api.Services;

public interface ITodoService
{
    Task<(IReadOnlyList<TodoItemDto> Items, int Total)> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct);
    Task<TodoItemDto?> GetByIdAsync(int id, CancellationToken ct);
    Task<TodoItemDto> CreateAsync(CreateTodoItemDto dto, CancellationToken ct);
    Task<bool> UpdateAsync(int id, UpdateTodoItemDto dto, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
