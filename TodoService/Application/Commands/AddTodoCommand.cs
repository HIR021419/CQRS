using MediatR;
using System.Runtime.Serialization;
using TodoService.Models;

namespace TodoService.Application.Commands
{
    public class AddTodoCommand : IRequest<TodoItem>
    {
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public string UserID { get; private set; }

        public AddTodoCommand(string name, string userId)
        {
            Name = name;
            UserID = userId;
        }
    }
}
