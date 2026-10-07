using Microsoft.EntityFrameworkCore;
using Example5WeekWeb.Models;

namespace Example5WeekWeb.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Example5WeekWeb.Models.TodoItem> Todos { get; set; }
    }
}
