using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoList.Server.DataAccess;
using TodoList.Server.Models;

namespace TodoList.Server.Application.Commands
{
    public class UpdateTodoHandler : IRequestHandler<UpdateTodoCommand, TodoItem?>
    {
        private readonly TodoDbContext _repository;

        public UpdateTodoHandler(TodoDbContext repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<TodoItem?> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
        {
            TodoItem? existingItem = await _repository.TodoItems.FirstOrDefaultAsync(item => item.Id == request.Item.Id, cancellationToken);
            if (existingItem == null) return null;
            existingItem.Name = request.Item.Name;
            existingItem.IsDone = request.Item.IsDone;
            await _repository.SaveChangesAsync(cancellationToken);
            return existingItem;
        }
    }
}
