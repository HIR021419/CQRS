using MediatR;
using UserService.Models;

namespace UserService.Application.Commands
{
    public class AddUserCommand(string name, string email) : IRequest<User>
    {
        public string Name { get; private set; } = name;
        public string Email { get; private set; } = name;
    }
}
