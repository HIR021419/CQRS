using MediatR;
using UserService.Models;

namespace UserService.Application.Queries
{
    public class GetAllUsersQuery : IRequest<IEnumerable<User>> { }
}
