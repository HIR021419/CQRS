using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoService.DataAccess;
using TodoService.Models;

namespace TodoService.Application.Commands
{
    public class UpdateTodoHandler : IRequestHandler<UpdateTodoCommand, TodoItem?>
    {
        private readonly TodoServiceContext _repository;

        public UpdateTodoHandler(TodoServiceContext repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<TodoItem?> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
        {
            TodoItem? existingItem = await _repository.TodoItems.Include(t => t.User).AsNoTracking().FirstOrDefaultAsync(item => item.Id == request.Item.Id, cancellationToken);
            if (existingItem == null) return null;
            existingItem.Name = request.Item.Name;
            existingItem.IsDone = request.Item.IsDone;
            await _repository.SaveChangesAsync(cancellationToken);
            return existingItem;
        }
    }
}
