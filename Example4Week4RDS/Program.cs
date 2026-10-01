
// Program.cs
// Entry point for the Example4Week4RDS sample app.
// - Registers services (controllers, EF Core DbContext)
// - Configures the HTTP pipeline (Swagger in Development, HTTPS, Authorization)
// - Demonstrates a simple development-time approach to ensure the database exists
//   and seed a few sample Todo items. For production use, prefer EF Migrations
//   or other deployment-time schema management.
namespace Example4Week4RDS
{
    using Microsoft.EntityFrameworkCore;
    using Example4Week4RDS.Data;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Configure EF Core to use SQL Server with a local connection string.
            // The connection string is read from appsettings.json under ConnectionStrings:DefaultConnection.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                                   ?? "Server=localhost\\SQLEXPRESS;Database=TodoDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

            builder.Services.AddDbContext<TodoContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Ensure database is created. For learning/demo purposes we use EnsureCreated so migrations are not required.
            // Note: EnsureCreated can create the database and schema automatically when the
            // connection string targets a SQL Server instance (local or remote) and the
            // account has sufficient privileges. It is not recommended for production
            // environments or when using EF Migrations.
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<TodoContext>();
                //db.Database.EnsureCreated();

                // Seed demo data when the table is empty. This is a simple in-app seeding
                // for learning purposes. In a production scenario, prefer scripted or
                // migration-based seeding during deployment.
                if (!db.TodoItems.Any())
                {
                    db.TodoItems.AddRange(
                        new TodoItem { Title = "Buy milk", IsCompleted = false, DueAt = DateTime.Now.AddDays(2) },
                        new TodoItem { Title = "Write blog post", IsCompleted = false, DueAt = DateTime.Now.AddDays(3) },
                        new TodoItem { Title = "Pay bills", IsCompleted = true, DueAt = null }
                    );
                    db.SaveChanges();
                }
            }

            // Configure the HTTP request pipeline.
            // In Development the app exposes the Swagger UI. You can view it at
            // <app-base-url>/swagger (for example: https://localhost:5001/swagger or http://localhost:5000/swagger)
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
