using Microsoft.EntityFrameworkCore;
using Amazon.Extensions.Configuration.SystemsManager;
using Amazon;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.Data.SqlClient;

namespace Example6WeekMVCWebRDS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Option A: (legacy) you can configure a path to load many parameters using
            // AWS Systems Manager. Historically an environment variable like
            // "AWS__SSMPath" (double underscore for colon) was used to map to
            // configuration key "AWS:SSMPath". That is why you may see "AWS__SSMPath".
            //
            // Option B (classic): load specific parameters for DB credentials from
            // Parameter Store. This sample reads two parameters (names configurable)
            // and injects them into the EF Core connection string.

            // Parameter names can be overridden via configuration keys or environment variables:
            // - AWS:DbUserParameter / AWS__DbUserParameter
            // - AWS:DbPasswordParameter / AWS__DbPasswordParameter
            var dbUserParamName = builder.Configuration["AWS:DbUserParameter"]
                                  ?? Environment.GetEnvironmentVariable("AWS__DbUserParameter")
                                  ?? "/Example6WeekMVCWebRDS/DbUser";
            var dbPasswordParamName = builder.Configuration["AWS:DbPasswordParameter"]
                                      ?? Environment.GetEnvironmentVariable("AWS__DbPasswordParameter")
                                      ?? "/Example6WeekMVCWebRDS/DbPassword";
            var dbServerParamName = builder.Configuration["AWS:DbServerParameter"]
                                      ?? Environment.GetEnvironmentVariable("AWS__DbServerParameter")
                                      ?? "/Example6WeekMVCWebRDS/DbServer";

            string dbUser = null;
            string dbPassword = null;
            string dbServer = null;
            try
            {
                // Resolve AWS region (allow override via config or env); default to Canada Central (ca-central-1)
                var awsRegion = builder.Configuration["AWS:Region"]
                                ?? Environment.GetEnvironmentVariable("AWS_REGION")
                                ?? Environment.GetEnvironmentVariable("AWS_DEFAULT_REGION")
                                ?? "ca-central-1";
                Console.WriteLine($"AWS region resolved: {awsRegion}");

                var ssmConfig = new AmazonSimpleSystemsManagementConfig
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(awsRegion)
                };

                using var ssm = new AmazonSimpleSystemsManagementClient(ssmConfig);
                var request = new GetParametersRequest
                {
                    Names = new List<string> { dbUserParamName, dbPasswordParamName, dbServerParamName },
                    WithDecryption = true
                };
                Console.WriteLine($"SSM: requesting parameters: {string.Join(", ", request.Names)}");
                var response = ssm.GetParametersAsync(request).GetAwaiter().GetResult();
                Console.WriteLine($"SSM: received {response.Parameters.Count} parameters");
                if (response.InvalidParameters != null && response.InvalidParameters.Count > 0)
                {
                    Console.WriteLine($"SSM: invalid/missing parameters: {string.Join(", ", response.InvalidParameters)}");
                }
                foreach (var p in response.Parameters)
                {
                    // Log parameter name and a masked value for sensitive parameters (do not print secrets)
                    if (string.Equals(p.Name, dbPasswordParamName, StringComparison.Ordinal))
                    {
                        var len = p.Value?.Length ?? 0;
                        Console.WriteLine($"SSM: parameter '{p.Name}' retrieved (masked) length={len}");
                    }
                    else
                    {
                        Console.WriteLine($"SSM: parameter '{p.Name}' = '{p.Value}'");
                    }

                    if (string.Equals(p.Name, dbUserParamName, StringComparison.Ordinal))
                        dbUser = p.Value;
                    if (string.Equals(p.Name, dbPasswordParamName, StringComparison.Ordinal))
                        dbPassword = p.Value;
                    if (string.Equals(p.Name, dbServerParamName, StringComparison.Ordinal))
                        dbServer = p.Value;
                }
                Console.WriteLine($"SSM: final resolved values -> dbUser={(dbUser ?? "null")}, dbServer={(dbServer ?? "null")}, dbPassword={(dbPassword != null ? "<present>" : "null")}");
            }
            catch (Exception ex)
            {
                // Reading SSM failed; continue using configuration sources already present.
                Console.WriteLine($"Warning: failed to read DB credentials from SSM: {ex}");
            }

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddControllersWithViews();

            // Add DbContext
            // db connection string is loading appsettings.json, but you can override it with an environment variable
            // default environment variable name is "ConnectionStrings__DefaultConnection" (double underscore)
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // If DB credentials were retrieved from SSM, inject them into the connection string.
            if (!string.IsNullOrEmpty(dbUser) && !string.IsNullOrEmpty(dbPassword) && !string.IsNullOrEmpty(connectionString))
            {
                try
                {
                    var csb = new SqlConnectionStringBuilder(connectionString);
                    csb.UserID = dbUser;
                    csb.Password = dbPassword;
                    // If a DB server/host was provided by SSM, override the DataSource in the connection string
                    if (!string.IsNullOrEmpty(dbServer))
                    {
                        csb.DataSource = dbServer;
                    }
                    // make sure integrated security / trusted connection is disabled when using SQL auth
                    csb.IntegratedSecurity = false;
                    connectionString = csb.ConnectionString;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: failed to update connection string with SSM credentials: {ex.Message}");
                }
            }

            builder.Services.AddDbContext<Example6WeekMVCWebRDS.Data.TodoContext>(options =>
                options.UseSqlServer(connectionString));

            var app = builder.Build();

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

            // Map conventional controller routes so controllers like TodoController
            // (which do not use attribute routing) are reachable at /{controller}/{action}/{id?}
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            // Seed database
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<Example6WeekMVCWebRDS.Data.TodoContext>();
                    Example6WeekMVCWebRDS.Data.DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }

            app.Run();
        }
    }
}
