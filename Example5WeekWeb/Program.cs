using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Example5WeekWeb.Data;

namespace Example5WeekWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            // Register In-Memory database for student study purposes
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("StudentsDb"));

            var app = builder.Build();

            // Seed sample data for study
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
                if (!db.Students.Any())
                {
                    db.Students.AddRange(
                        new Models.Student { Name = "Alice", EnrollmentDate = DateTime.UtcNow.AddMonths(-3) },
                        new Models.Student { Name = "Bob", EnrollmentDate = DateTime.UtcNow.AddMonths(-1) }
                    );
                    db.SaveChanges();
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
