using MediatR;
using System.Collections;
using System.Linq.Expressions;
using TodoList.Server.Models;

namespace TodoList.Server.Application.Queries
{
    public class GetAllTodosQuery : IRequest<IEnumerable<TodoItem>> { }
}
