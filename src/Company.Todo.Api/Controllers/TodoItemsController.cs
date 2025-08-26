using Company.Todo.Api.Dtos;
using Company.Todo.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Company.Todo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoItemsController(ITodoService service) : ControllerBase
{
    private readonly ITodoService _service = service;

    [HttpGet("Get")]
    public async Task<ActionResult<object>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var (items, total) = await _service.GetPagedAsync(page, pageSize, search, ct);
        return Ok(new { total, items });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoItemDto>> GetById([FromRoute] int id, CancellationToken ct)
    {
        var item = await _service.GetByIdAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost("Create")]
    public async Task<ActionResult<TodoItemDto>> Create([FromBody] CreateTodoItemDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Title)) return BadRequest("Title is required.");
        var created = await _service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("Update")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateTodoItemDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Title)) return BadRequest("Title is required.");
        var ok = await _service.UpdateAsync(id, dto, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
    {
        var ok = await _service.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }

    // protected endpoint if SSO is enabled
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            user = User.Identity?.Name ?? "anonymous",
            claims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }
}
