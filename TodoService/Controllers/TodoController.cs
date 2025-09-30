using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoService.Application.Commands;
using TodoService.Application.Queries;
using TodoService.Models;

namespace TodoList.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController(IMediator mediator) : Controller
    {
        private  readonly IMediator _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<TodoItem>> AddTodo([FromBody] AddTodoCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.Name))
                return BadRequest("Name and UserID are required.");

            var result = await _mediator.Send(command);
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
