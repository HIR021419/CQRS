using MediatR;
using System.Runtime.Serialization;
using TodoList.Server.Models;

namespace TodoList.Server.Application.Commands
{
    public class AddTodoCommand : IRequest<TodoItem>
    {
        [DataMember]
        public string Name { get; private set; }

        public AddTodoCommand(string name) => Name = name;
    }
}
