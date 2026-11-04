
using Example8WeekWebAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace Example8WeekWebAPI
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
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

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
