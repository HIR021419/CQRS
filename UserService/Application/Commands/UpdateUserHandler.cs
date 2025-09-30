using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.DataAccess;
using UserService.Models;
using UserService.Services;

namespace UserService.Application.Commands
{
    public class UpdateUserHandler(UserDbContext repository) : IRequestHandler<UpdateUserCommand, User?>
    {
        private readonly UserDbContext _repository = repository;
        private readonly MessagePublisher _publisher = MessagePublisher.GetInstance();

        public async Task<User?> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _repository.Users.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

            if (user == null) return null;

            user.Name = request.Name;
            user.Email = request.Email;

            await _repository.SaveChangesAsync(cancellationToken);

            await _publisher.Publish("user.update", user);

            return user;
        }
    }
}
