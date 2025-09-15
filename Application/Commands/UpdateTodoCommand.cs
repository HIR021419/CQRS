using MediatR;
using System.Runtime.Serialization;
using TodoList.Server.Models;

namespace TodoList.Server.Application.Commands
{
    public class UpdateTodoCommand : IRequest<TodoItem?>
    {
        [DataMember]
        public TodoItem Item { get; private set; }

        public UpdateTodoCommand(TodoItem item) => Item = item;
    }
}
