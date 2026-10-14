using Example6WeekMVCWebRDS.Models;
using Microsoft.EntityFrameworkCore;

namespace Example6WeekMVCWebRDS.Data
{
    public static class DbInitializer
    {
        public static void Initialize(TodoContext context)
        {
            // Use EF Core migrations at runtime to ensure schema is up-to-date.
            // This applies any pending migrations. For development/demo purposes
            // this is convenient; for production consider running migrations from
            // deployment pipelines instead.
            context.Database.Migrate();

            if (context.TodoItems.Any())
            {
                return; // DB has been seeded
            }

            var todos = new TodoItem[]
            {
                new TodoItem { Title = "Buy groceries", IsCompleted = false, DueAt = DateTime.UtcNow.AddDays(2) },
                new TodoItem { Title = "Walk the dog", IsCompleted = true, DueAt = DateTime.UtcNow.AddDays(1) },
                new TodoItem { Title = "Finish report", IsCompleted = false, DueAt = null }
            };

            context.TodoItems.AddRange(todos);
            context.SaveChanges();
        }
    }
}
