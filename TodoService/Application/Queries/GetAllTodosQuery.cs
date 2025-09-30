using MediatR;
using TodoService.Models;

namespace TodoService.Application.Queries
{
    public class GetAllTodosQuery : IRequest<IEnumerable<TodoItem>> { }
}
