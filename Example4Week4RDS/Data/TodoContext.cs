using Microsoft.EntityFrameworkCore;

namespace Example4Week4RDS.Data
{
    // EF Core DbContext for the Todo sample application.
    // Registered in Program.cs with a SQL Server provider and a connection string.
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options) : base(options)
        {
        }

        // Represents the TodoItems table in the database.
        public DbSet<TodoItem> TodoItems { get; set; } = null!;
    }
}
