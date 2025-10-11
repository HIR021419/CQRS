using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TodoService.DataAccess;
using TodoService.Models;

namespace TodoService.Application.Commands
{
    public class AddTodoHandler : IRequestHandler<AddTodoCommand, TodoItem>
    {
        private readonly TodoServiceContext _repository;

        public AddTodoHandler(TodoServiceContext repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<TodoItem> Handle(AddTodoCommand request, CancellationToken cancellationToken)
        {
            User? user = await _repository.Users.FindAsync(new Guid(request.UserID), cancellationToken);
            if (user == null) throw new Exception("invalid user id");
            TodoItem item = new TodoItem(request.Name, user);
            _repository.Add(item);
            await _repository.SaveChangesAsync(cancellationToken);
            return item;
        }
    }
}
