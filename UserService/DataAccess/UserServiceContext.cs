using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.DataAccess
{
    public class UserServiceContext : DbContext
    {
        public UserServiceContext(DbContextOptions<UserServiceContext> options) : base(options) { }
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<IntegrationEvent> IntegrationEventOutbox { get; set; } = null!;
    }
}
