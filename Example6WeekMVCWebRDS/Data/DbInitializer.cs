using System;
using System.Linq;
using Example6WeekMVCWebRDS.Models;
using Microsoft.EntityFrameworkCore;

namespace Example6WeekMVCWebRDS.Data
{
    public static class DbInitializer
    {
        public static void Initialize(TodoContext context)
        {
            // Use EF Core migrations at runtime to ensure schema is up-to-date when
            // migrations are present. If there are no migrations in the project
            // (e.g. running against a fresh DB without any migration files),
            // fall back to EnsureCreated() so the schema is created from the model.
            // Note: EnsureCreated() is not compatible with Migrations and is
            // intended only for simple development/demo scenarios.
            var hasMigrations = context.Database.GetMigrations().Any();
            if (hasMigrations)
            {
                context.Database.Migrate();
            }
            else
            {
                context.Database.EnsureCreated();
            }

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
