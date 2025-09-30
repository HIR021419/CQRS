using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.DataAccess;
using UserService.Models;

namespace UserService.Application.Queries
{
    public class GetAllUsersHandler(UserDbContext repository) : IRequestHandler<GetAllUsersQuery, IEnumerable<User>>
    {
        private readonly UserDbContext _repository = repository;

        public async Task<IEnumerable<User>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            return await _repository.Users.AsNoTracking().ToListAsync(cancellationToken);
        }
    }
}
