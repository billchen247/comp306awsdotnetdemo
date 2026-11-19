using Example10WeekTodoApi.Data;
using Microsoft.EntityFrameworkCore;

namespace Example10WeekTodoApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Add DbContext with in-memory database
            builder.Services.AddDbContext<TodoDbContext>(options =>
                options.UseInMemoryDatabase("TodoDb"));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            // ECS / ALB health checks
            app.MapGet("/helath", () => Results.Ok(new { 
                                                        status = "Healthy", 
                                                        timestamp = DateTime.UtcNow 
                                                        }));
            

            // Home page with useful links
            app.MapGet("/", () => Results.Content(
                """
                <!DOCTYPE html>
                <html lang="en">
                <head>
                    <meta charset="utf-8" />
                    <meta name="viewport" content="width=device-width, initial-scale=1" />
                    <title>Example10WeekTodoApi</title>
                    <style>
                        body { font-family: Arial, sans-serif; margin: 2rem; line-height: 1.6; }
                        h1 { margin-bottom: .5rem; }
                        ul { padding-left: 1.2rem; }
                        li { margin: .35rem 0; }
                    </style>
                </head>
                <body>
                    <h1>Example10WeekTodoApi</h1>
                    <p>Available endpoints:</p>
                    <ul>
                        <li><a href="/swagger">Swagger UI</a></li>
                        <li><a href="/helath">Health Check (/helath)</a></li>
                        <li><a href="/health">Health Check (/health)</a></li>
                        <li><a href="/api/Todo">Todo API</a></li>
                        <li><a href="/WeatherForecast">Weather Forecast API</a></li>
                    </ul>
                </body>
                </html>
                """,
                "text/html"));

            // Seed the database with initial data
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
                TodoSeeder.SeedTodos(dbContext);
            }

            app.Run();
        }
    }
}
