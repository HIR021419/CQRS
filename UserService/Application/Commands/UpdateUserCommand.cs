using MediatR;
using UserService.Models;

namespace UserService.Application.Commands
{
    public class UpdateUserCommand(Guid id, string name, string email) : IRequest<User?>
    {
        public Guid Id { get; } = id;
        public string Name { get; } = name;
        public string Email { get; } = email;
    }
}
