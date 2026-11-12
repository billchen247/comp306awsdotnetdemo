# Quick Start Guide - CityInfo API

## 5-Minute Setup

### Step 1: Configure Database Connection
Edit `Example9WeekCityInfoAPI/appsettings.json`:
```json
"ConnectionStrings": {
  "CityInfoConnection": "Server=(localdb)\\mssqllocaldb;Database=CityInfoDb;Trusted_Connection=true;"
}
```

Or for SQL Server:
```json
"ConnectionStrings": {
  "CityInfoConnection": "Server=YOUR_SERVER;Database=CityInfoDb;User Id=sa;Password=YOUR_PASSWORD;"
}
```

### Step 2: Create Database
```bash
cd Example9WeekCityInfoLibrary
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Step 3: Run the API
```bash
cd ../Example9WeekCityInfoAPI
dotnet run
```

Expected output:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7000
```

### Step 4: Open Swagger UI
Navigate to: **https://localhost:7000/swagger**

## API Endpoints Quick Reference

### Cities
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/cities` | Get all cities |
| GET | `/api/cities/{id}` | Get city by ID |
| POST | `/api/cities` | Create city |
| PUT | `/api/cities/{id}` | Update city (full) |
| PATCH | `/api/cities/{id}` | Update city (partial) |
| DELETE | `/api/cities/{id}` | Delete city |

### Points of Interest
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/cities/{cid}/pointsofinterest` | Get all POIs |
| GET | `/api/cities/{cid}/pointsofinterest/{id}` | Get POI by ID |
| POST | `/api/cities/{cid}/pointsofinterest` | Create POI |
| PUT | `/api/cities/{cid}/pointsofinterest/{id}` | Update POI (full) |
| PATCH | `/api/cities/{cid}/pointsofinterest/{id}` | Update POI (partial) |
| DELETE | `/api/cities/{cid}/pointsofinterest/{id}` | Delete POI |

## Test with cURL

### Create a City
```bash
curl -X POST "https://localhost:7000/api/cities" ^
  -H "Content-Type: application/json" ^
  -d "{\"cityName\": \"New York\", \"description\": \"The Big Apple\"}"
```

### Get All Cities
```bash
curl -X GET "https://localhost:7000/api/cities" ^
  -H "accept: application/json"
```

### Create Point of Interest
```bash
curl -X POST "https://localhost:7000/api/cities/1/pointsofinterest" ^
  -H "Content-Type: application/json" ^
  -d "{\"nameofPOI\": \"Times Square\", \"description\": \"Famous intersection\"}"
```

### Partial Update with JSON Patch
```bash
curl -X PATCH "https://localhost:7000/api/cities/1" ^
  -H "Content-Type: application/json-patch+json" ^
  -d "[{\"op\": \"replace\", \"path\": \"/description\", \"value\": \"Updated description\"}]"
```

## Postman Setup

1. Open Postman
2. Create new collection: "CityInfo API"
3. Add environment variable:
   - `base_url` = `https://localhost:7000`
   - `cityId` = `1`

4. Create requests:
   - **GET All Cities**: `{{base_url}}/api/cities`
   - **Create City**: `POST {{base_url}}/api/cities`
   - **Update City**: `PUT {{base_url}}/api/cities/{{cityId}}`

## Project Files

| File | Purpose |
|------|---------|
| `README.md` | Complete documentation |
| `ARCHITECTURE.md` | Detailed architecture patterns |
| `API_EXAMPLES.http` | HTTP examples for all endpoints |
| `IMPLEMENTATION_SUMMARY.md` | What was built (this summary) |

## Troubleshooting

### Issue: "Cannot connect to database"
- Check connection string in `appsettings.json`
- Verify SQL Server is running
- Check firewall settings

### Issue: "Port 7000 already in use"
- Use different port: `dotnet run --urls="https://localhost:7001"`

### Issue: "Swagger page doesn't load"
- Check that project is running
- Verify HTTPS certificate is trusted
- Try incognito/private window

### Issue: "Migration fails"
- Delete existing database and recreate
- Use `dotnet ef database drop` then `dotnet ef database update`

## Next Steps

1. ? Review `README.md` for full documentation
2. ? Check `ARCHITECTURE.md` for design patterns
3. ? Test endpoints in Swagger UI
4. ? Review `Controllers/` for code examples
5. ? Customize DTOs for your needs
6. ? Add authentication if needed
7. ? Deploy to your hosting platform

## Key Features at a Glance

? **RESTful API** - Standard REST conventions
? **JSON Patch** - Partial updates (RFC 6902)
? **Async/Await** - Non-blocking operations
? **AutoMapper** - Clean object mapping
? **Repository Pattern** - Data access abstraction
? **Swagger UI** - Interactive API documentation
? **Error Handling** - Proper HTTP status codes
? **Logging** - Built-in operation tracking

## Architecture Overview

```
Request ? Controller ? AutoMapper ? Repository ? DbContext ? Database
                ?
            Validation
                ?
            Logging
```

1. **Controller** - Handles HTTP requests/responses
2. **AutoMapper** - Maps DTOs to/from entities
3. **Repository** - Handles data access
4. **DbContext** - EF Core data model
5. **Database** - SQL Server storage

## File Structure

```
Example9WeekCityInfoAPI/
??? Controllers/          # API endpoints
??? DTOs/                 # Data transfer objects
??? Profiles/             # AutoMapper profiles
??? Repositories/         # Data access layer
??? Program.cs            # DI configuration
??? appsettings.json      # Configuration
??? README.md             # Full documentation
??? ARCHITECTURE.md       # Design patterns
??? API_EXAMPLES.http     # HTTP examples
```

## Support

For detailed information:
- See `README.md` for complete API documentation
- See `ARCHITECTURE.md` for design pattern explanations
- See `API_EXAMPLES.http` for endpoint examples
- Check controller code for implementation details

---

**Status**: ? Ready to Use  
**Last Updated**: 2024  
**Framework**: ASP.NET Core 8  
**Database**: SQL Server (via Entity Framework Core)
