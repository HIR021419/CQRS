using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoService.DataAccess;
using TodoService.Models;

namespace TodoService.Application.Queries
{
    public class GetAllTodosHandler : IRequestHandler<GetAllTodosQuery, IEnumerable<TodoItem>>
    {
        private readonly TodoDbContext _repository;

        public GetAllTodosHandler(TodoDbContext repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<IEnumerable<TodoItem>> Handle(GetAllTodosQuery request, CancellationToken cancellationToken)
        {
            return await _repository.TodoItems.Include(t => t.User).AsNoTracking().ToListAsync(cancellationToken);
        }
    }
}
