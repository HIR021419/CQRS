using MediatR;
using System.Net;
using TodoList.Server.DataAccess;
using TodoList.Server.Models;

namespace TodoList.Server.Application.Commands
{
    public class AddTodoHandler : IRequestHandler<AddTodoCommand, TodoItem>
    {
        private readonly TodoDbContext _repository;
        public AddTodoHandler(TodoDbContext repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        public async Task<TodoItem> Handle(AddTodoCommand request, CancellationToken cancellationToken)
        {
            var item = new TodoItem(request.Name);
            _repository.Add(item);
            await _repository.SaveChangesAsync(cancellationToken);
            return item;
        }
    }
}
