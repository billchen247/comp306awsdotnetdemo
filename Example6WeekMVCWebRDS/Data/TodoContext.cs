using Microsoft.EntityFrameworkCore;
using Example6WeekMVCWebRDS.Models;

namespace Example6WeekMVCWebRDS.Data
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options) : base(options)
        {
        }

        public DbSet<TodoItem> Todos { get; set; } = null!;
    }
}
