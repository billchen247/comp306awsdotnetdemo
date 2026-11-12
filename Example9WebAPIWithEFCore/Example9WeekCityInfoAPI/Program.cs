
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

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
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
