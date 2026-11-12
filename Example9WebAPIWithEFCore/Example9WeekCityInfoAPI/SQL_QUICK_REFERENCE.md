# ?? Quick SQL Server & Seeding Setup

## TL;DR - Get Running in 30 Seconds

### Step 1: Run the API
```bash
cd Example9WeekCityInfoAPI
dotnet run
```

### Step 2: Watch Console Output
```
Starting database seeding...
Successfully seeded database with 5 cities and their points of interest.
Now listening on: https://localhost:7000
```

### Step 3: Test API
Open: `https://localhost:7000/swagger`

**Done!** ? Your database is created and populated with sample data.

---

## Connection String Reference

### Using LocalDB (Default - No Setup Needed)
```json
"CityInfoConnection": "Server=(localdb)\\mssqllocaldb;Database=CityInfoDb;Trusted_Connection=true;"
```

### Using SQL Server Express
```json
"CityInfoConnection": "Server=.\\SQLEXPRESS;Database=CityInfoDb;Trusted_Connection=true;"
```

### Using SQL Server with Username/Password
```json
"CityInfoConnection": "Server=localhost;Database=CityInfoDb;User Id=sa;Password=YourPassword123!;"
```

**File**: `appsettings.json`

---

## Common Tasks

### See All Cities
```bash
curl https://localhost:7000/api/cities
```

### See City with POIs
```bash
curl https://localhost:7000/api/cities/1
```

### Reset Database
```bash
# Delete database
sqlcmd -S (localdb)\mssqllocaldb
DROP DATABASE CityInfoDb;
GO
exit

# Then restart app
dotnet run
```

### Disable Auto-Seeding
Edit `Program.cs` and comment out the seeding section (see `SQL_SERVER_SETUP.md`)

---

## What Gets Seeded

**5 Cities**:
- New York (5 POIs)
- Los Angeles (4 POIs)
- San Francisco (4 POIs)
- Chicago (4 POIs)
- Boston (4 POIs)

**Total**: 21 Points of Interest

**Seeding Logic**:
- ? Creates database if missing
- ? Creates tables from models
- ? Checks if data exists
- ? Only seeds if empty
- ? Logs all actions

---

## Troubleshooting

| Problem | Solution |
|---------|----------|
| "Cannot connect" | Check connection string in `appsettings.json` |
| "Database locked" | Close SQL tools, restart VS, try again |
| "Seeding failed" | Check logs in Output window, verify DB permissions |
| "Data not appearing" | Refresh browser, check `/api/cities` endpoint |

---

## Files Involved

| File | Purpose |
|------|---------|
| `appsettings.json` | Connection string |
| `Program.cs` | Startup configuration |
| `Data/CityInfoDbSeeder.cs` | Seeding implementation |
| `Data/SeedingOptions.cs` | Seeding configuration |

---

## Full Documentation

See `SQL_SERVER_SETUP.md` for:
- Detailed connection string options
- Environment-specific configuration
- Manual seeding instructions
- Advanced troubleshooting
- Customizing seed data

---

**Ready to go!** ??

Just run `dotnet run` and everything else happens automatically.
