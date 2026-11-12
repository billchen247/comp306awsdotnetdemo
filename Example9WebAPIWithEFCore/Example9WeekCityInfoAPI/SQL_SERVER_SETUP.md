# Local SQL Server Setup Guide

## Overview

This guide explains how to set up a local SQL Server connection and automatically seed data when your CityInfo API starts.

---

## Part 1: SQL Server Connection String Setup

### Option 1: SQL Server LocalDB (Recommended for Development)

**LocalDB** is a lightweight version of SQL Server Express included with Visual Studio.

#### Connection String
```json
"ConnectionStrings": {
  "CityInfoConnection": "Server=(localdb)\\mssqllocaldb;Database=CityInfoDb;Trusted_Connection=true;"
}
```

**Location**: `Example9WeekCityInfoAPI/appsettings.json`

**Current Configuration** (Already Set):
```json
{
  "ConnectionStrings": {
    "CityInfoConnection": "Server=(localdb)\\mssqllocaldb;Database=CityInfoDb;Trusted_Connection=true;"
  }
}
```

#### Why LocalDB?
- ? No installation required (comes with Visual Studio)
- ? Lightweight and fast
- ? Perfect for development
- ? Automatic cleanup
- ? No server to manage

---

### Option 2: SQL Server Express (Local Installation)

If you have SQL Server Express installed locally:

```json
"ConnectionStrings": {
  "CityInfoConnection": "Server=.;Database=CityInfoDb;Trusted_Connection=true;"
}
```

Or with a named instance:
```json
"ConnectionStrings": {
  "CityInfoConnection": "Server=.\\SQLEXPRESS;Database=CityInfoDb;Trusted_Connection=true;"
}
```

---

### Option 3: SQL Server Developer Edition

For a full-featured local SQL Server:

```json
"ConnectionStrings": {
  "CityInfoConnection": "Server=.;Database=CityInfoDb;Trusted_Connection=true;Encrypt=false;"
}
```

---

### Option 4: With SQL Authentication (Username/Password)

If you need to use SQL authentication instead of Windows authentication:

```json
"ConnectionStrings": {
  "CityInfoConnection": "Server=.;Database=CityInfoDb;User Id=sa;Password=YourPassword123!;Encrypt=false;"
}
```

---

## Part 2: Automatic Database Seeding

### How It Works

When you run `dotnet run`, the application will:

1. **Create the database** if it doesn't exist
2. **Create tables** from your models
3. **Check if data exists** - If yes, skip seeding
4. **Seed sample data** - Add 5 cities with 17 points of interest
5. **Start the API** - Ready to use

### Seeder Features

**File**: `Example9WeekCityInfoAPI/Data/CityInfoDbSeeder.cs`

```csharp
public class CityInfoDbSeeder
{
    // Seeds 5 cities with their points of interest
    public async Task SeedAsync()
    {
        // Checks if data exists
        if (await _context.Cities.AnyAsync())
        {
            return; // Skip if already seeded
        }
        
        // Adds sample data
        await _context.Cities.AddRangeAsync(cities);
        await _context.SaveChangesAsync();
    }
}
```

### Sample Data Included

#### Cities (5 Total)
1. **New York** - 5 POIs
   - Statue of Liberty
   - Central Park
   - Empire State Building
   - Times Square
   - Brooklyn Bridge

2. **Los Angeles** - 4 POIs
   - Hollywood Walk of Fame
   - Griffith Observatory
   - Venice Beach
   - Getty Center

3. **San Francisco** - 4 POIs
   - Golden Gate Bridge
   - Alcatraz Island
   - Cable Cars
   - Fisherman's Wharf

4. **Chicago** - 4 POIs
   - Willis Tower
   - Millennium Park
   - Art Institute of Chicago
   - Navy Pier

5. **Boston** - 4 POIs
   - Freedom Trail
   - Boston Common
   - Museum of Fine Arts
   - Faneuil Hall and Quincy Market

**Total**: 5 cities + 21 points of interest

---

## Part 3: Getting Started

### Step 1: Verify Connection String

Open `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "CityInfoConnection": "Server=(localdb)\\mssqllocaldb;Database=CityInfoDb;Trusted_Connection=true;"
  }
}
```

**For LocalDB**: No changes needed ?

**For SQL Server Express**: Change to:
```json
"CityInfoConnection": "Server=.\\SQLEXPRESS;Database=CityInfoDb;Trusted_Connection=true;"
```

### Step 2: Run the Application

```bash
cd Example9WeekCityInfoAPI
dotnet run
```

**Expected Output**:
```
info: Example9WeekCityInfoAPI.Data.CityInfoDbSeeder[0]
      Starting database seeding...
info: Example9WeekCityInfoAPI.Data.CityInfoDbSeeder[0]
      Successfully seeded database with 5 cities and their points of interest.
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7000
```

### Step 3: Verify with Swagger

Open: `https://localhost:7000/swagger`

Click **Authorize** (if using auth), then test:

1. **GET** `/api/cities` - Should return 5 cities
2. **GET** `/api/cities/1` - Should show New York with 5 POIs
3. **GET** `/api/cities/1/pointsofinterest` - Should list all POIs

---

## Part 4: Environment-Specific Configuration

### Development Settings

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

### Production Settings

**File**: `appsettings.Production.json` (Create this file)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Warning"
    }
  },
  "ConnectionStrings": {
    "CityInfoConnection": "Server=YOUR_PROD_SERVER;Database=CityInfoDb;User Id=sa;Password=YourSecurePassword!;Encrypt=true;"
  }
}
```

**Set Environment Before Running**:
```bash
# Windows
set ASPNETCORE_ENVIRONMENT=Development

# Linux/Mac
export ASPNETCORE_ENVIRONMENT=Development

# Then run
dotnet run
```

---

## Part 5: Connection String Components Explained

### LocalDB Connection String
```
Server=(localdb)\mssqllocaldb;Database=CityInfoDb;Trusted_Connection=true;
```

| Component | Meaning |
|-----------|---------|
| `Server=(localdb)\mssqllocaldb` | Connect to LocalDB instance |
| `Database=CityInfoDb` | Use CityInfoDb database |
| `Trusted_Connection=true` | Use Windows authentication |

### SQL Server Express Connection String
```
Server=.\SQLEXPRESS;Database=CityInfoDb;Trusted_Connection=true;
```

| Component | Meaning |
|-----------|---------|
| `Server=.\SQLEXPRESS` | Local computer, SQLEXPRESS instance |
| `Database=CityInfoDb` | Use CityInfoDb database |
| `Trusted_Connection=true` | Use Windows authentication |

### SQL Authentication Connection String
```
Server=localhost;Database=CityInfoDb;User Id=sa;Password=Password123!;
```

| Component | Meaning |
|-----------|---------|
| `Server=localhost` | Local server |
| `Database=CityInfoDb` | Use CityInfoDb database |
| `User Id=sa` | SQL login username |
| `Password=Password123!` | SQL login password |

---

## Part 6: Troubleshooting

### Issue: "Cannot connect to database"

**Error Message**:
```
SqlException: A network-related or instance-specific error occurred...
```

**Solutions**:
1. Verify connection string is correct
2. Ensure SQL Server is running
3. Check database name matches
4. Verify network connectivity

**Test Connection**:
```bash
sqlcmd -S (localdb)\mssqllocaldb
```

### Issue: "Database already exists with different schema"

**Error Message**:
```
Violation of PRIMARY KEY constraint
```

**Solution**: Delete the existing database and restart
```bash
# In SQL Server Management Studio or sqlcmd
DROP DATABASE CityInfoDb;
```

Then run the API again to recreate and reseed.

### Issue: "Permission denied"

**Error Message**:
```
Login failed for user...
```

**Solutions**:
1. Verify Windows credentials
2. Check SQL Server login permissions
3. Use `Trusted_Connection=true` for Windows auth
4. Use `User Id` and `Password` for SQL auth

### Issue: "Seeding fails but API starts"

**Error Message**:
```
An error occurred while seeding the database during startup.
```

**Why**: The app is designed to continue even if seeding fails (production-safe)

**Solution**:
1. Check logs in Output window
2. Verify database permissions
3. Manually seed using Package Manager Console:
```powershell
dotnet ef database update
```

---

## Part 7: Viewing Seeded Data

### Using SQL Server Management Studio

```sql
-- View all cities
SELECT * FROM Cities;

-- View all points of interest
SELECT * FROM PointsOfInterest;

-- View city with its POIs
SELECT c.CityName, p.NameofPOI, p.Description
FROM Cities c
LEFT JOIN PointsOfInterest p ON c.CityId = p.CityId
ORDER BY c.CityName, p.NameofPOI;
```

### Using the API

**Get all cities**:
```bash
curl https://localhost:7000/api/cities
```

**Get city with POIs**:
```bash
curl https://localhost:7000/api/cities/1
```

**Get all POIs for a city**:
```bash
curl https://localhost:7000/api/cities/1/pointsofinterest
```

---

## Part 8: Disabling Auto-Seeding

If you want to prevent automatic seeding, comment out the seeding code in `Program.cs`:

```csharp
// Seed the database on startup
// using (var scope = app.Services.CreateScope())
// {
//     var seeder = scope.ServiceProvider.GetRequiredService<CityInfoDbSeeder>();
//     try
//     {
//         await seeder.SeedAsync();
//     }
//     catch (Exception ex)
//     {
//         var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
//         logger.LogError(ex, "An error occurred while seeding the database during startup.");
//     }
// }
```

---

## Part 9: Customizing Seed Data

To add or modify seed data, edit `CityInfoDbSeeder.cs`:

```csharp
var cities = new List<CityInfo>
{
    new CityInfo
    {
        CityName = "Your City",
        Description = "Your description",
        PointsOfInterest = new List<PointsOfInterest>
        {
            new PointsOfInterest
            {
                NameofPOI = "Your POI",
                Description = "POI description"
            }
        }
    }
};
```

Then delete the database and restart the app.

---

## Part 10: Resetting the Database

### Complete Reset

1. **Delete the database**:
```bash
sqlcmd -S (localdb)\mssqllocaldb
DROP DATABASE CityInfoDb;
GO
exit
```

2. **Restart the API**:
```bash
dotnet run
```

The database will be recreated and reseeded automatically.

### Via Visual Studio

1. Open **View ? SQL Server Object Explorer**
2. Expand **(localdb)\mssqllocaldb**
3. Right-click on **CityInfoDb**
4. Select **Delete**
5. Restart the API

---

## Summary

? **Connection String**: Already configured for LocalDB  
? **Auto-Seeding**: Implemented and runs on startup  
? **Sample Data**: 5 cities + 21 POIs  
? **Error Handling**: Gracefully continues if seeding fails  
? **Environment Support**: Dev, staging, production ready  

**To Start**:
```bash
dotnet run
```

That's it! The database will be created, seeded, and your API will be ready to use.

---

**For more information**, see:
- `appsettings.json` - Connection string configuration
- `Data/CityInfoDbSeeder.cs` - Seeding implementation
- `Program.cs` - Startup configuration
