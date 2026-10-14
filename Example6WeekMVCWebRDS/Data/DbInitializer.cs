using Example6WeekMVCWebRDS.Models;

namespace Example6WeekMVCWebRDS.Data
{
    public static class DbInitializer
    {
        public static void Initialize(TodoContext context)
        {
            context.Database.EnsureCreated();

            if (context.Todos.Any())
            {
                return; // DB has been seeded
            }

            var todos = new TodoItem[]
            {
                new TodoItem { Title = "Buy groceries", IsDone = false },
                new TodoItem { Title = "Walk the dog", IsDone = true },
                new TodoItem { Title = "Finish report", IsDone = false }
            };

            context.Todos.AddRange(todos);
            context.SaveChanges();
        }
    }
}
