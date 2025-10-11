using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoService.Application.Commands;
using TodoService.Application.Queries;
using TodoService.Models;
using Microsoft.EntityFrameworkCore;
using TodoService.DataAccess;

namespace TodoService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly TodoServiceContext _context;

        public TodoController(IMediator mediator, TodoServiceContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<TodoItem>> AddTodo([FromBody] AddTodoCommand command)
        {
            Guid userId = new Guid(command.UserID);
            if (string.IsNullOrWhiteSpace(command.Name) || userId == Guid.Empty)
                return BadRequest("Name and UserId are required.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return BadRequest("Associated User does not exist in local database.");

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TodoItem>> Update(Guid id, [FromBody] TodoItem item)
        {
            if (id != item.Id)
                return BadRequest("Mismatched Todo ID");

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

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetById(Guid id)
        {
            var item = await _context.TodoItems.FindAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }
    }
}
