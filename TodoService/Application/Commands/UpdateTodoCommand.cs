using MediatR;
using System.Runtime.Serialization;
using TodoService.Models;

namespace TodoService.Application.Commands
{
    public class UpdateTodoCommand : IRequest<TodoItem?>
    {
        [DataMember]
        public TodoItem Item { get; private set; }

        public UpdateTodoCommand(TodoItem item) => Item = item;
    }
}
