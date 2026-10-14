using Microsoft.EntityFrameworkCore;
using Example6WeekMVCWebRDS.Models;

namespace Example6WeekMVCWebRDS.Data
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options) : base(options)
        {
        }

        // Match the existing database table name `TodoItems`.
        public DbSet<TodoItem> TodoItems { get; set; } = null!;
    }
}
