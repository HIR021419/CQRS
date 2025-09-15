using Microsoft.AspNetCore.Mvc;
using MediatR;
using TodoList.Server.Application.Commands;
using TodoList.Server.Application.Queries;
using TodoList.Server.Models;

namespace TodoList.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        private  readonly IMediator _mediator;

        public TodoController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<TodoItem>> AddTodo([FromBody] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Name is required.");

            var result = await _mediator.Send(new AddTodoCommand(name));
            return CreatedAtAction(nameof(GetAll), result);
        }

        [HttpPut]
        public async Task<ActionResult<TodoItem>> Update([FromBody] TodoItem item)
        {
            var updated = await _mediator.Send(new UpdateTodoCommand(item));
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetAll()
        {
            var items = await _mediator.Send(new GetAllTodosQuery());
            return Ok(items);
        }
    }
}
