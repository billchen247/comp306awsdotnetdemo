using Example8WeekWebAPI.Models;

namespace Example8WeekWebAPI.Data
{
    public static class TodoSeeder
    {
        public static void SeedTodos(TodoDbContext context)
        {
            // Only seed if database is empty
            if (context.Todos.Any())
            {
                return;
            }

            var todos = new List<Todo>
            {
                new Todo
                {
                    Title = "Learn ASP.NET Core",
                    Description = "Study ASP.NET Core fundamentals and best practices",
                    IsComplete = false,
                    CreatedAt = DateTime.UtcNow
                },
                new Todo
                {
                    Title = "Build a REST API",
                    Description = "Create a complete REST API with CRUD operations",
                    IsComplete = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new Todo
                {
                    Title = "Write Unit Tests",
                    Description = "Write comprehensive unit tests for the API",
                    IsComplete = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new Todo
                {
                    Title = "Deploy to Azure",
                    Description = "Deploy the application to Azure App Service",
                    IsComplete = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                },
                new Todo
                {
                    Title = "Add Authentication",
                    Description = "Implement JWT authentication for the API",
                    IsComplete = false,
                    CreatedAt = DateTime.UtcNow
                }
            };

            context.Todos.AddRange(todos);
            context.SaveChanges();
        }
    }
}
