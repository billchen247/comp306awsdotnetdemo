
using Example9WeekCityInfoAPI.Data;
using Example9WeekCityInfoAPI.Profiles;
using Example9WeekCityInfoAPI.Repositories;
using Example9WeekCityInfoLibrary.Data;
using Microsoft.EntityFrameworkCore;

namespace Example9WeekCityInfoAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container

            // Database configuration
            // db connection string is loading appsettings.json, but you can override it with an environment variable
            // default environment variable name is "ConnectionStrings__DefaultConnection" (double underscore)
            // Use DefaultConnection2Docker (host.docker.internal with SQL auth) when running in Docker, otherwise use DefaultConnection

            // Support connection string from:
            // 1. Environment variable (CITYINFO_CONNECTION_STRING)
            // 2. appsettings.json (ConnectionStrings:CityInfoConnection)
            var connectionString = Environment.GetEnvironmentVariable("CITYINFO_CONNECTION_STRING") 
                ?? builder.Configuration.GetConnectionString("CityInfoConnection");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'CityInfoConnection' not found in configuration or environment variable 'CITYINFO_CONNECTION_STRING'.");
            }

            builder.Services.AddDbContext<CityInfoDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Register repository
            builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();

            // Register database seeder
            builder.Services.AddScoped<CityInfoDbSeeder>();

            // Register AutoMapper
            builder.Services.AddAutoMapper(typeof(CityInfoProfile));

            // Add controllers with JSON Patch support
            builder.Services.AddControllers()
                .AddNewtonsoftJson();

            // Swagger/OpenAPI configuration
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "CityInfo API",
                    Version = "v1",
                    Description = "API for managing cities and points of interest"
                });

                // Add XML documentation
                var xmlFile = "Example9WeekCityInfoAPI.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });

            var app = builder.Build();

            // ECS / ALB health checks
            app.MapGet("/health", () => Results.Ok(new
            {
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
                    <title>Example8WeekWebAPI</title>
                    <style>
                        body { font-family: Arial, sans-serif; margin: 2rem; line-height: 1.6; }
                        h1 { margin-bottom: .5rem; }
                        ul { padding-left: 1.2rem; }
                        li { margin: .35rem 0; }
                    </style>
                </head>
                <body>
                    <h1>Example8WeekWebAPI</h1>
                    <p>Available endpoints:</p>
                    <ul>
                        <li><a href="/swagger">Swagger UI</a></li>
                        <li><a href="/helath">Health Check (/helath)</a></li>
                        <li><a href="/api/cities">cities API</a></li>
                        <li><a href="/api/Todo">Todo API</a></li>
                        <li><a href="/WeatherForecast">Weather Forecast API</a></li>
                    </ul>
                </body>
                </html>
                """,
                "text/html"));


            

            // Seed the database on startup
            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<CityInfoDbSeeder>();
                try
                {
                    await seeder.SeedAsync();
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database during startup.");
                    // Don't throw - allow the app to continue even if seeding fails
                }
            }

            // Configure the HTTP request pipeline for all env
            // the path to access the swagger UI is /swagger/index.html
            if (app.Environment.IsDevelopment() || true)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CityInfo API V1");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}
