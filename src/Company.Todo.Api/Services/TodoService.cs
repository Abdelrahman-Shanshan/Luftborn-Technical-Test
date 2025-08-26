using Company.Todo.Api.Dtos;
using Company.Todo.Api.Models;
using Company.Todo.Api.Repositories;

namespace Company.Todo.Api.Services;

public class TodoService(IUnitOfWork uow) : ITodoService
{
    private readonly IUnitOfWork _uow = uow;

    public async Task<(IReadOnlyList<TodoItemDto> Items, int Total)> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct)
    {
        var items = await _uow.Todos.ListPagedAsync(page, pageSize, search, ct);
        var total = await _uow.Todos.CountAsync(search, ct);
        return (items.Select(ToDto).ToList(), total);
    }

    public async Task<TodoItemDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var entity = await _uow.Todos.GetByIdAsync(id, ct);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<TodoItemDto> CreateAsync(CreateTodoItemDto dto, CancellationToken ct)
    {
        var entity = new TodoItem
        {
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim()
        };
        await _uow.Todos.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<bool> UpdateAsync(int id, UpdateTodoItemDto dto, CancellationToken ct)
    {
        var entity = await _uow.Todos.GetByIdAsync(id, ct);
        if (entity is null) return false;
        entity.Title = dto.Title.Trim();
        entity.Description = dto.Description?.Trim();
        if (!entity.IsCompleted && dto.IsCompleted)
            entity.CompletedAtUtc = DateTime.UtcNow;
        entity.IsCompleted = dto.IsCompleted;
        _uow.Todos.Update(entity);
        await _uow.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await _uow.Todos.GetByIdAsync(id, ct);
        if (entity is null) return false;
        _uow.Todos.Remove(entity);
        await _uow.SaveChangesAsync(ct);
        return true;
    }

    private static TodoItemDto ToDto(TodoItem e) => new()
    {
        Id = e.Id,
        Title = e.Title,
        Description = e.Description,
        IsCompleted = e.IsCompleted,
        CreatedAtUtc = e.CreatedAtUtc,
        CompletedAtUtc = e.CompletedAtUtc
    };
}
