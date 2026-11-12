# Complete Setup Guide - Local SQL Server & Database Seeding

## ?? Overview

This guide covers:
1. ? Local SQL Server connection setup
2. ? Automatic database seeding on startup
3. ? Sample data (5 cities + 21 POIs)
4. ? Environment-specific configuration
5. ? Troubleshooting and customization

---

## ?? Part 1: SQL Server Connection Setup

### Current Configuration

Your project is **already configured** for LocalDB:

**File**: `appsettings.json`
```json
{
  "ConnectionStrings": {
    "CityInfoConnection": "Server=(localdb)\\mssqllocaldb;Database=CityInfoDb;Trusted_Connection=true;"
  }
}
```

### What is LocalDB?

- **LocalDB** is a lightweight SQL Server version included with Visual Studio
- Perfect for development
- No installation or configuration needed
- Automatically creates `.mdf` files in your user folder
- Automatically cleans up when you're done

### How to Use LocalDB

**No setup required!** Just run:
```bash
dotnet run
```

The system will automatically:
1. Create the LocalDB instance (if not exists)
2. Create the `CityInfoDb` database
3. Create tables from your models
4. Seed sample data
5. Start your API

---

## ??? Part 2: Alternative SQL Server Configurations

If you prefer a different SQL Server setup, update `appsettings.json`:

### Option A: SQL Server Express (If Installed)

```json
{
  "ConnectionStrings": {
    "CityInfoConnection": "Server=.\\SQLEXPRESS;Database=CityInfoDb;Trusted_Connection=true;"
  }
}
```

### Option B: SQL Server Developer Edition

```json
{
  "ConnectionStrings": {
    "CityInfoConnection": "Server=.;Database=CityInfoDb;Trusted_Connection=true;Encrypt=false;"
  }
}
```

### Option C: SQL Authentication (Username/Password)

```json
{
  "ConnectionStrings": {
    "CityInfoConnection": "Server=localhost;Database=CityInfoDb;User Id=sa;Password=YourSecurePassword!;"
  }
}
```

### Option D: Remote SQL Server

```json
{
  "ConnectionStrings": {
    "CityInfoConnection": "Server=192.168.1.100;Database=CityInfoDb;User Id=sa;Password=YourPassword!;Encrypt=true;"
  }
}
```

### Option E: Azure SQL Database

```json
{
  "ConnectionStrings": {
    "CityInfoConnection": "Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=CityInfoDb;Persist Security Info=False;User ID=sqladmin@yourserver;Password=YourPassword!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

---

## ?? Part 3: Automatic Database Seeding

### How It Works

When you start the API, the `CityInfoDbSeeder` class:

1. **Creates database** - If `CityInfoDb` doesn't exist
2. **Creates tables** - From your DbContext models
3. **Checks for data** - If data exists, skip seeding
4. **Adds sample data** - 5 cities with 21 POIs
5. **Logs progress** - See what's happening in console

### Seeder Implementation

**File**: `Example9WeekCityInfoAPI/Data/CityInfoDbSeeder.cs`

```csharp
public class CityInfoDbSeeder
{
    public async Task SeedAsync()
    {
        // Ensure database exists
        await _context.Database.EnsureCreatedAsync();

        // Skip if data already exists
        if (await _context.Cities.AnyAsync())
        {
            return;
        }

        // Add sample cities and POIs
        await _context.Cities.AddRangeAsync(cities);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Database seeding completed successfully.");
    }
}
```

### Sample Data Included

#### Cities Seeded (5 Total)

| City | Description | POIs |
|------|-------------|------|
| **New York** | The Big Apple | Statue of Liberty, Central Park, Empire State Building, Times Square, Brooklyn Bridge |
| **Los Angeles** | City of Angels | Hollywood Walk of Fame, Griffith Observatory, Venice Beach, Getty Center |
| **San Francisco** | City by the Bay | Golden Gate Bridge, Alcatraz Island, Cable Cars, Fisherman's Wharf |
| **Chicago** | Windy City | Willis Tower, Millennium Park, Art Institute, Navy Pier |
| **Boston** | The Hub | Freedom Trail, Boston Common, Museum of Fine Arts, Faneuil Hall |

**Total**: 21 Points of Interest

---

## ?? Part 4: Getting Started (Step by Step)

### Step 1: Verify Connection String

Open `Example9WeekCityInfoAPI/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "CityInfoConnection": "Server=(localdb)\\mssqllocaldb;Database=CityInfoDb;Trusted_Connection=true;"
  }
}
```

- ? For **LocalDB**: No changes needed
- ?? For other servers: Replace connection string (see Part 2)

### Step 2: Run the Application

```bash
cd Example9WeekCityInfoAPI
dotnet run
```

### Step 3: Watch the Console Output

```
info: Example9WeekCityInfoAPI.Data.CityInfoDbSeeder[0]
      Starting database seeding...

info: Example9WeekCityInfoAPI.Data.CityInfoDbSeeder[0]
      Successfully seeded database with 5 cities and their points of interest.

info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7000
      Now listening on: http://localhost:5000
```

### Step 4: Open Swagger UI

Navigate to: **https://localhost:7000/swagger**

### Step 5: Test the API

**Get all cities**:
```bash
GET /api/cities
```

**Expected Response** (200 OK):
```json
[
  {
    "cityId": 1,
    "cityName": "New York",
    "description": "The Big Apple - A vibrant metropolis...",
    "pointsOfInterestCount": 5,
    "pointsOfInterest": [...]
  },
  ...
]
```

**Get specific city**:
```bash
GET /api/cities/1
```

**Get points of interest**:
```bash
GET /api/cities/1/pointsofinterest
```

---

## ?? Part 5: Startup Configuration Details

### Program.cs Seeding Setup

**File**: `Example9WeekCityInfoAPI/Program.cs`

```csharp
public static async Task Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    // ... service registration ...

    var app = builder.Build();

    // Seed database on startup
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
            logger.LogError(ex, "Seeding failed - app will continue anyway");
        }
    }

    // ... rest of pipeline ...
    await app.RunAsync();
}
```

**Key Points**:
- ? Main is now `async Task` instead of `void`
- ? Creates a service scope for seeding
- ? Catches seeding errors (app continues even if seeding fails)
- ? Logs all seeding activities
- ? Uses `await app.RunAsync()` for proper async handling

### Service Registration

```csharp
// Register the seeder
builder.Services.AddScoped<CityInfoDbSeeder>();

// Register DbContext
builder.Services.AddDbContext<CityInfoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CityInfoConnection")));

// Register Repository
builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();
```

---

## ?? Part 6: Environment-Specific Configuration

### Development Configuration

**File**: `appsettings.Development.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

### Production Configuration

**Create**: `appsettings.Production.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "CityInfoConnection": "Server=your-prod-server.com;Database=CityInfoDb;User Id=produser;Password=SecurePassword!;Encrypt=true;"
  }
}
```

### Staging Configuration

**Create**: `appsettings.Staging.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "CityInfoConnection": "Server=staging-server.com;Database=CityInfoDb_Staging;Trusted_Connection=true;"
  }
}
```

### Setting Environment

**Windows Command Prompt**:
```batch
set ASPNETCORE_ENVIRONMENT=Development
dotnet run
```

**Windows PowerShell**:
```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run
```

**Linux/Mac**:
```bash
export ASPNETCORE_ENVIRONMENT=Development
dotnet run
```

**Visual Studio**:
- Select from dropdown in debug settings

**Visual Studio Code**:
Add to `.vscode/launch.json`:
```json
{
  "name": ".NET Core Launch",
  "env": {
    "ASPNETCORE_ENVIRONMENT": "Development"
  }
}
```

---

## ?? Part 7: Viewing Seeded Data

### Via Swagger UI

1. Open `https://localhost:7000/swagger`
2. Click **GET /api/cities**
3. Click **Try it out**
4. Click **Execute**
5. See all 5 seeded cities in response

### Via CURL Commands

**Get all cities**:
```bash
curl -X GET "https://localhost:7000/api/cities" -H "accept: application/json"
```

**Get single city**:
```bash
curl -X GET "https://localhost:7000/api/cities/1" -H "accept: application/json"
```

**Get all POIs**:
```bash
curl -X GET "https://localhost:7000/api/cities/1/pointsofinterest" -H "accept: application/json"
```

### Via SQL Server Management Studio

```sql
-- View cities
SELECT * FROM Cities;

-- View POIs
SELECT * FROM PointsOfInterest;

-- View cities with POIs
SELECT 
    c.CityId,
    c.CityName,
    p.PointsOfInterestId,
    p.NameofPOI,
    p.Description
FROM Cities c
LEFT JOIN PointsOfInterest p ON c.CityId = p.CityId
ORDER BY c.CityName, p.NameofPOI;
```

### Via Code

```csharp
var cities = await _repository.GetAllCitiesAsync();
foreach (var city in cities)
{
    Console.WriteLine($"City: {city.CityName} ({city.PointsOfInterest.Count} POIs)");
    foreach (var poi in city.PointsOfInterest)
    {
        Console.WriteLine($"  - {poi.NameofPOI}");
    }
}
```

---

## ??? Part 8: Customizing Seed Data

### Adding More Cities

Edit `Example9WeekCityInfoAPI/Data/CityInfoDbSeeder.cs`:

```csharp
var cities = new List<CityInfo>
{
    // Existing cities...
    
    // Add your new city
    new CityInfo
    {
        CityName = "Denver",
        Description = "The Mile High City",
        PointsOfInterest = new List<PointsOfInterest>
        {
            new PointsOfInterest
            {
                NameofPOI = "Red Rocks Park",
                Description = "Natural amphitheatre for concerts and events"
            },
            new PointsOfInterest
            {
                NameofPOI = "Denver Art Museum",
                Description = "World-class art museum"
            }
        }
    }
};
```

### Rebuilding Seed Data

1. Delete the existing database:
```bash
sqlcmd -S (localdb)\mssqllocaldb
DROP DATABASE CityInfoDb;
GO
exit
```

2. Restart the app:
```bash
dotnet run
```

The new seed data will be created.

---

## ?? Part 9: Troubleshooting

### Issue: "Cannot connect to database"

**Error**:
```
SqlException: A network-related or instance-specific error occurred...
```

**Solutions**:
1. Verify connection string in `appsettings.json`
2. Check SQL Server is running
3. Verify database name is correct
4. Check credentials (if using SQL auth)

**Test Connection**:
```bash
sqlcmd -S (localdb)\mssqllocaldb
SELECT @@VERSION;
GO
exit
```

### Issue: "Seeding completed but no data appears"

**Possible causes**:
1. API started before seeding completed
2. Wrong database selected
3. Seeding encountered an error

**Check logs**:
- Look at Visual Studio Output window
- Search for "seeding" messages
- Check for error messages

**Manual verification**:
```bash
curl https://localhost:7000/api/cities
```

Should return JSON array. If empty array `[]`:
- Database was created but not seeded
- Check seeding logs for errors

### Issue: "Database already exists"

**Error**:
```
The database 'CityInfoDb' already exists...
```

**Solutions**:
1. **Keep existing data**: Application will use it as-is
2. **Delete and recreate**:
```bash
sqlcmd -S (localdb)\mssqllocaldb
DROP DATABASE CityInfoDb;
GO
exit

# Then restart app
dotnet run
```

### Issue: "Permission denied"

**Error**:
```
Login failed for user 'DOMAIN\username'
```

**Solutions**:
1. Check Windows user has SQL Server permissions
2. Use SQL authentication instead of Windows auth
3. Run Visual Studio as Administrator
4. Check firewall settings

### Issue: "Timeout during seeding"

**Error**:
```
Timeout expired. The timeout period elapsed...
```

**Solutions**:
1. Increase network timeout in connection string:
```json
"CityInfoConnection": "Server=(localdb)\\mssqllocaldb;Database=CityInfoDb;Trusted_Connection=true;Connection Timeout=60;"
```

2. Check system resources
3. Verify database isn't locked by another application
4. Check SQL Server logs

---

## ?? Part 10: Monitoring Seeding

### Console Logging

The seeder logs important information:

```
info: Example9WeekCityInfoAPI.Data.CityInfoDbSeeder[0]
      Starting database seeding...

info: Example9WeekCityInfoAPI.Data.CityInfoDbContext[0]
      Created database for context 'CityInfoDbContext'.

info: Example9WeekCityInfoAPI.Data.CityInfoDbSeeder[0]
      Successfully seeded database with 5 cities and their points of interest.
```

### Checking Seeding Status

In `Program.cs`, you can add custom logging:

```csharp
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<CityInfoDbSeeder>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    logger.LogInformation("Starting database seeding...");
    try
    {
        await seeder.SeedAsync();
        logger.LogInformation("Database seeding completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database seeding failed");
    }
}
```

---

## ?? Part 11: File Structure

```
Example9WeekCityInfoAPI/
??? Program.cs                      # Main startup with seeding
??? appsettings.json                # Connection string (LocalDB)
??? appsettings.Development.json    # Dev logging config
??? appsettings.Production.json     # Production config (create if needed)
?
??? Data/
?   ??? CityInfoDbSeeder.cs         # Seeding implementation
?   ??? SeedingOptions.cs           # Seeding configuration
?
??? Controllers/
?   ??? CitiesController.cs
?   ??? PointsOfInterestController.cs
?
??? DTOs/
?   ??? CityInfoDto.cs
?   ??? CityInfoCreateDto.cs
?   ??? ... (other DTOs)
?
??? Repositories/
?   ??? ICityInfoRepository.cs
?   ??? CityInfoRepository.cs
?
??? Profiles/
?   ??? CityInfoProfile.cs
?
??? Documentation/
    ??? SQL_SERVER_SETUP.md         # This file
    ??? SQL_QUICK_REFERENCE.md      # Quick reference
    ??? README.md                   # API documentation
```

---

## ? Verification Checklist

Before assuming everything is working:

- [ ] `appsettings.json` has correct connection string
- [ ] API starts with `dotnet run`
- [ ] Console shows "Starting database seeding..."
- [ ] Console shows "Successfully seeded database..."
- [ ] Swagger UI loads at `https://localhost:7000/swagger`
- [ ] `GET /api/cities` returns 5 cities
- [ ] `GET /api/cities/1` returns New York
- [ ] `GET /api/cities/1/pointsofinterest` returns 5 POIs
- [ ] No errors in Visual Studio Output window

---

## ?? Quick Commands Reference

```bash
# Run the application
dotnet run

# Run with specific environment
set ASPNETCORE_ENVIRONMENT=Development && dotnet run

# Test connection
sqlcmd -S (localdb)\mssqllocaldb

# List databases
sqlcmd -S (localdb)\mssqllocaldb -Q "SELECT name FROM sys.databases;"

# View database
sqlcmd -S (localdb)\mssqllocaldb -d CityInfoDb -Q "SELECT * FROM Cities;"

# Delete database
sqlcmd -S (localdb)\mssqllocaldb -Q "DROP DATABASE CityInfoDb;"
```

---

## ?? Next Steps

1. ? Review connection string in `appsettings.json`
2. ? Run `dotnet run` to start seeding
3. ? Open `https://localhost:7000/swagger` to test API
4. ? Review `SQL_QUICK_REFERENCE.md` for common tasks
5. ? Explore `Repositories/` to understand data access
6. ? Check `Controllers/` to see API implementation

---

**Your database is now automatically created and seeded on startup!** ??

For quick reference, see `SQL_QUICK_REFERENCE.md`
