using MediatR;
using UserService.DataAccess;
using UserService.Models;
using UserService.Services;

namespace UserService.Application.Commands
{
    public class AddUserHandler(UserDbContext repository) : IRequestHandler<AddUserCommand, User>
    {
        private readonly UserDbContext _repository = repository;

        public async Task<User> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            User user = new(request.Name, request.Email);

            _repository.Users.Add(user);
            await _repository.SaveChangesAsync(cancellationToken);

            await MessagePublisher.GetInstance().Publish("user.add", user);

            return user;
        }
    }
}
