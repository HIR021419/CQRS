using Microsoft.EntityFrameworkCore;
using TodoList.Server.Models;

namespace TodoList.Server.DataAccess
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) {}
        public DbSet<TodoItem> TodoItems { get; set; } = null!;
    }
}
