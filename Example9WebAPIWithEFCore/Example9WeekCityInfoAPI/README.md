# CityInfo REST API

A comprehensive REST API for managing cities and points of interest built with ASP.NET Core 8, Entity Framework Core, and following modern best practices.

## Architecture Overview

This API follows the classic MVC architecture with the following components:

### 1. **Data Models** (in Example9WeekCityInfoLibrary)
- `CityInfo` - Represents a city entity
- `PointsOfInterest` - Represents a point of interest within a city

### 2. **Data Transfer Objects (DTOs)**
Transfer objects are used for API requests/responses to decouple API contracts from domain models:

**City DTOs:**
- `CityInfoDto` - City data (read)
- `CityInfoCreateDto` - City creation payload
- `CityInfoUpdateDto` - City update payload

**Points of Interest DTOs:**
- `PointsOfInterestDto` - POI data (read)
- `PointsOfInterestCreateDto` - POI creation payload
- `PointsOfInterestUpdateDto` - POI update payload

### 3. **Repository Pattern**
- `ICityInfoRepository` - Interface defining data access contracts
- `CityInfoRepository` - Implementation handling all database operations

**Key Benefits:**
- Abstraction of data access logic
- Easy to test (mock the repository)
- Centralized data access patterns
- Consistent error handling

### 4. **AutoMapper Profiles**
- `CityInfoProfile` - Defines mappings between entities and DTOs
- Automatic property mapping with support for complex relationships

### 5. **Controllers**
RESTful API endpoints implementing CRUD operations:

#### CitiesController (`/api/cities`)
- `GET /api/cities` - Get all cities
- `GET /api/cities/{cityId}` - Get a specific city with POIs
- `POST /api/cities` - Create a new city
- `PUT /api/cities/{cityId}` - Full update of a city
- `PATCH /api/cities/{cityId}` - Partial update using JSON Patch
- `DELETE /api/cities/{cityId}` - Delete a city

#### PointsOfInterestController (`/api/cities/{cityId}/pointsofinterest`)
- `GET /api/cities/{cityId}/pointsofinterest` - Get all POIs for a city
- `GET /api/cities/{cityId}/pointsofinterest/{poiId}` - Get a specific POI
- `POST /api/cities/{cityId}/pointsofinterest` - Create a new POI
- `PUT /api/cities/{cityId}/pointsofinterest/{poiId}` - Full update of a POI
- `PATCH /api/cities/{cityId}/pointsofinterest/{poiId}` - Partial update using JSON Patch
- `DELETE /api/cities/{cityId}/pointsofinterest/{poiId}` - Delete a POI

## Key Features

### JSON Patch Support
Full support for RFC 6902 JSON Patch operations:

```json
[
  { "op": "replace", "path": "/description", "value": "New description" },
  { "op": "add", "path": "/tags", "value": ["tag1", "tag2"] },
  { "op": "remove", "path": "/deprecated" }
]
```

### Error Handling
- Comprehensive HTTP status codes (200, 201, 204, 400, 404)
- Consistent error response format
- Model validation with automatic error messages

### Logging
Integrated logging for:
- API operations
- Database queries (in development)
- Warnings and errors

### API Documentation
- Swagger/OpenAPI integration for interactive API exploration
- XML documentation comments on all public members
- Detailed endpoint descriptions and response types

## Setup and Configuration

### 1. Connection String
Update `appsettings.json` with your database connection:

```json
{
  "ConnectionStrings": {
    "CityInfoConnection": "Server=your-server;Database=CityInfoDb;Trusted_Connection=true;"
  }
}
```

### 2. Database Migration
Create and apply migrations using Entity Framework Core:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 3. Run the API
```bash
dotnet run
```

The API will be available at `https://localhost:7000` (or configured port)
Swagger UI: `https://localhost:7000/swagger`

## API Examples

### Get All Cities
```bash
curl -X GET "https://localhost:7000/api/cities" -H "accept: application/json"
```

### Create a City
```bash
curl -X POST "https://localhost:7000/api/cities" \
  -H "Content-Type: application/json" \
  -d '{
    "cityName": "New York",
    "description": "The Big Apple"
  }'
```

### Full Update (PUT)
```bash
curl -X PUT "https://localhost:7000/api/cities/1" \
  -H "Content-Type: application/json" \
  -d '{
    "cityName": "New York City",
    "description": "Updated description"
  }'
```

### Partial Update (PATCH)
```bash
curl -X PATCH "https://localhost:7000/api/cities/1" \
  -H "Content-Type: application/json-patch+json" \
  -d '[
    { "op": "replace", "path": "/description", "value": "Patched description" }
  ]'
```

### Create Point of Interest
```bash
curl -X POST "https://localhost:7000/api/cities/1/pointsofinterest" \
  -H "Content-Type: application/json" \
  -d '{
    "nameofPOI": "Statue of Liberty",
    "description": "Famous monument"
  }'
```

### Delete
```bash
curl -X DELETE "https://localhost:7000/api/cities/1" \
  -H "accept: */*"
```

## Project Structure

```
Example9WeekCityInfoAPI/
??? Controllers/
?   ??? CitiesController.cs
?   ??? PointsOfInterestController.cs
??? DTOs/
?   ??? CityInfoDto.cs
?   ??? CityInfoCreateDto.cs
?   ??? CityInfoUpdateDto.cs
?   ??? PointsOfInterestDto.cs
?   ??? PointsOfInterestCreateDto.cs
?   ??? PointsOfInterestUpdateDto.cs
??? Profiles/
?   ??? CityInfoProfile.cs
??? Repositories/
?   ??? ICityInfoRepository.cs
?   ??? CityInfoRepository.cs
??? Program.cs
??? appsettings.json
??? appsettings.Development.json

Example9WeekCityInfoLibrary/
??? Models/
?   ??? CityInfo.cs
?   ??? PointsOfInterest.cs
??? Data/
    ??? CityInfoDbContext.cs
```

## NuGet Packages

- **AutoMapper** - Object-to-object mapping
- **AutoMapper.Extensions.Microsoft.DependencyInjection** - DI integration
- **Microsoft.AspNetCore.JsonPatch** - JSON Patch support
- **Microsoft.AspNetCore.Mvc.NewtonsoftJson** - Newtonsoft.Json integration
- **Microsoft.EntityFrameworkCore** - ORM framework
- **Microsoft.EntityFrameworkCore.SqlServer** - SQL Server provider
- **Swashbuckle.AspNetCore** - Swagger/OpenAPI generation

## Best Practices Implemented

1. **Separation of Concerns** - Controllers, Services, Repositories
2. **SOLID Principles**
   - Single Responsibility - Each class has one reason to change
   - Open/Closed - Open for extension, closed for modification
   - Liskov Substitution - Repository interface used everywhere
   - Interface Segregation - Focused repository interface
   - Dependency Inversion - Depend on abstractions

3. **Async/Await** - All I/O operations are asynchronous
4. **Resource-Based URLs** - Hierarchical REST endpoints
5. **Proper HTTP Methods** - GET, POST, PUT, PATCH, DELETE
6. **Proper HTTP Status Codes** - 200, 201, 204, 400, 404
7. **DTOs for API Contracts** - Decoupled from domain models
8. **Logging** - Track application behavior
9. **Dependency Injection** - Loose coupling throughout

## Security Considerations

Current implementation is focused on architecture demonstration. For production:

- Add authentication (JWT, OAuth2)
- Add authorization (role-based access control)
- Implement rate limiting
- Add input validation more rigorously
- Use HTTPS exclusively
- Add CORS configuration if needed
- Implement request/response encryption if sensitive data is handled

## Testing

The repository pattern makes unit testing straightforward:

```csharp
// Mock the repository in tests
var mockRepository = new Mock<ICityInfoRepository>();
mockRepository.Setup(r => r.GetCityByIdAsync(1))
    .ReturnsAsync(new CityInfo { CityId = 1, CityName = "Test" });

var controller = new CitiesController(mockRepository.Object, mapper, logger);
```

## Future Enhancements

- Add filtering and pagination
- Add sorting capabilities
- Add search functionality
- Implement caching (Redis)
- Add API versioning
- Add request/response compression
- Add monitoring and metrics
