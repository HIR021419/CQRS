using Microsoft.EntityFrameworkCore;
using TodoService.Models;

namespace TodoService.DataAccess
{
    public class TodoServiceContext : DbContext
    {
        public TodoServiceContext(DbContextOptions<TodoServiceContext> options) : base(options) {}
        public DbSet<TodoItem> TodoItems { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
    }
}
