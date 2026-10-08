using Microsoft.EntityFrameworkCore;

namespace comp306sec402week5livedemo.Data
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options) : base(options) { }

        public DbSet<Models.TodoItem> TodoItems { get; set; } = null!;
    }
}
