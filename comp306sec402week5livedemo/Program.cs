
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using comp306sec402week5livedemo.Data;
using comp306sec402week5livedemo.Models;

namespace comp306sec402week5livedemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Register InMemory EF Core DB for Todo API
            builder.Services.AddDbContext<comp306sec402week5livedemo.Data.TodoContext>(options =>
                options.UseInMemoryDatabase("TodoDb"));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            // Seed initial data for Todo API
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<TodoContext>();

                // Ensure database is created (in-memory provider will handle this)
                context.Database.EnsureCreated();

                if (!context.TodoItems.Any())
                {
                    context.TodoItems.AddRange(
                        new TodoItem { Title = "Buy milk", IsComplete = false },
                        new TodoItem { Title = "Do homework", IsComplete = false },
                        new TodoItem { Title = "Walk the dog", IsComplete = true }
                    );
                    context.SaveChanges();
                }
            }


            // Configure the HTTP request pipeline.
            // Enable Swagger UI for all environments so the API explorer is always available.
            app.UseSwagger();
            app.UseSwaggerUI();

            // Home page for the API server
            app.MapGet("/", () =>
            {
                var html = @"<!doctype html>
<html lang=""en"">
  <head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
    <title>Todo API - Home</title>
    <style>
      body { font-family: Arial, Helvetica, sans-serif; margin: 2rem; }
      a { color: #0366d6; }
    </style>
  </head>
  <body>
    <h1>Todo API</h1>
    <p>This is a simple REST API for managing Todo items.</p>
    <ul>
      <li><a href=""/swagger/index.html"">Open Swagger UI</a></li>
      <li><a href=""/todo"">List Todo items (GET /todo)</a></li>
      <li><a href=""/todo/1"">Get Todo item with id=1 (GET /todo/1)</a></li>
    </ul>
    <p>Use the links above to explore the API.</p>
  </body>
</html>";

                return Results.Content(html, "text/html");
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
