# CityInfo API - Implementation Details and Architecture Notes

## Overview

This document provides detailed information about the architecture decisions, implementation patterns, and best practices used in the CityInfo API.

## Architecture Pattern: Repository Pattern

### Why Repository Pattern?

The Repository Pattern provides several key benefits:

1. **Abstraction** - Hide database implementation details
2. **Testability** - Mock the repository in unit tests
3. **Maintainability** - Centralize data access logic
4. **Consistency** - Uniform data access patterns
5. **Flexibility** - Easy to switch data sources (SQL, NoSQL, etc.)

### Implementation Structure

```
ICityInfoRepository (Interface)
    ??? CityInfoRepository (Implementation)
        ??? CityInfoDbContext (EF Core)
            ??? Database
```

### Key Methods

#### City Operations
- `GetAllCitiesAsync()` - Retrieves all cities ordered by name
- `GetCityByIdAsync(int cityId)` - Efficient single city lookup
- `GetCityWithPointsOfInterestAsync(int cityId)` - Eager loads related POIs
- `CityExistsAsync(int cityId)` - Quick existence check
- `AddCityAsync(CityInfo city)` - Queues new city for insertion
- `UpdateCityAsync(CityInfo city)` - Marks city for update
- `DeleteCityAsync(CityInfo city)` - Marks city for deletion
- `SaveChangesAsync()` - Persists all changes to database

#### Points of Interest Operations
- Similar structure for POI management
- Ensures data consistency with city references
- Supports parent-child relationships

## Data Transfer Objects (DTOs)

### Why DTOs?

1. **API Contract** - Define what clients can send/receive
2. **Decoupling** - Database models can change without breaking API
3. **Security** - Don't expose internal fields
4. **Validation** - Define which fields are required
5. **Flexibility** - Different DTOs for different operations

### DTO Classification

#### Read DTOs
- `CityInfoDto` - Returned on GET requests
- `PointsOfInterestDto` - Represents full entity data

#### Create DTOs
- `CityInfoCreateDto` - POST request payload
- `PointsOfInterestCreateDto` - POST request payload
- Don't include `Id` field (server generates)

#### Update DTOs
- `CityInfoUpdateDto` - PUT/PATCH request payload
- `PointsOfInterestUpdateDto` - PUT/PATCH request payload
- Don't include `Id` field (in URL)

### AutoMapper Configuration

```csharp
CreateMap<CityInfo, CityInfoDto>();
CreateMap<CityInfoCreateDto, CityInfo>();
CreateMap<CityInfoUpdateDto, CityInfo>();
CreateMap<CityInfo, CityInfoUpdateDto>();
```

- One-way mapping for entity ? DTO (no reverse)
- Two-way mapping for DTO ? entity for updates
- Bidirectional for PATCH operations

## REST API Design Principles

### Resource-Based URLs

```
/api/cities                           # Collection
/api/cities/{cityId}                  # Specific resource
/api/cities/{cityId}/pointsofinterest # Nested collection
/api/cities/{cityId}/pointsofinterest/{poiId} # Nested resource
```

**Benefits:**
- Intuitive hierarchical structure
- Clear parent-child relationships
- RESTful conventions followed

### HTTP Methods

| Method | Purpose | Idempotent | Safe |
|--------|---------|-----------|------|
| GET | Retrieve data | Yes | Yes |
| POST | Create new | No | No |
| PUT | Full replace | Yes | No |
| PATCH | Partial update | No* | No |
| DELETE | Remove | Yes | No |

*PATCH can be idempotent depending on operations

### HTTP Status Codes

| Code | Meaning | Usage |
|------|---------|-------|
| 200 | OK | Successful GET |
| 201 | Created | Successful POST (resource created) |
| 204 | No Content | Successful PUT/PATCH/DELETE |
| 400 | Bad Request | Invalid input/validation error |
| 404 | Not Found | Resource doesn't exist |
| 500 | Server Error | Unhandled exception |

### Response Location Header

On successful POST requests (201 Created):
```
Location: /api/cities/3
```

Allows clients to immediately GET the created resource.

## JSON Patch (RFC 6902) Implementation

### Supported Operations

1. **Add**
```json
{ "op": "add", "path": "/propertyName", "value": "newValue" }
```

2. **Remove**
```json
{ "op": "remove", "path": "/propertyName" }
```

3. **Replace**
```json
{ "op": "replace", "path": "/propertyName", "value": "newValue" }
```

4. **Move**
```json
{ "op": "move", "from": "/path1", "path": "/path2" }
```

5. **Copy**
```json
{ "op": "copy", "from": "/path1", "path": "/path2" }
```

6. **Test**
```json
{ "op": "test", "path": "/propertyName", "value": "expectedValue" }
```

### Implementation in Controller

```csharp
[HttpPatch("{cityId}")]
public async Task<IActionResult> PartiallyUpdateCity(
    int cityId,
    [FromBody] JsonPatchDocument<CityInfoUpdateDto> patchDocument)
{
    // 1. Get entity from database
    var city = await _repository.GetCityByIdAsync(cityId);
    if (city == null) return NotFound();
    
    // 2. Map entity to DTO
    var cityToPatch = _mapper.Map<CityInfoUpdateDto>(city);
    
    // 3. Apply patch operations
    patchDocument.ApplyTo(cityToPatch, ModelState);
    
    // 4. Validate result
    if (!ModelState.IsValid) return BadRequest(ModelState);
    
    // 5. Map back to entity
    _mapper.Map(cityToPatch, city);
    
    // 6. Save to database
    await _repository.UpdateCityAsync(city);
    await _repository.SaveChangesAsync();
    
    return NoContent();
}
```

## Async/Await Pattern

### Why Async?

1. **Scalability** - Handle more concurrent requests
2. **Performance** - Don't block threads on I/O operations
3. **Responsiveness** - Better user experience
4. **Resources** - More efficient thread usage

### Implementation

```csharp
// Controller methods are async
[HttpGet]
public async Task<ActionResult<IEnumerable<CityInfoDto>>> GetCities()
{
    // Repository methods are async
    var cities = await _repository.GetAllCitiesAsync();
    return Ok(_mapper.Map<IEnumerable<CityInfoDto>>(cities));
}

// Repository methods use async EF Core
public async Task<IEnumerable<CityInfo>> GetAllCitiesAsync()
{
    return await _context.Cities.ToListAsync();
}
```

### Best Practices

- Always await async operations
- Use `.Result` or `.Wait()` only in console apps
- Configure `.ConfigureAwait(false)` in libraries (not needed in web APIs)
- Never block on async (leads to deadlocks)

## Dependency Injection (DI)

### Service Registration in Program.cs

```csharp
// Database context
builder.Services.AddDbContext<CityInfoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CityInfoConnection")));

// Repository
builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(CityInfoProfile));

// Controllers with JSON Patch
builder.Services.AddControllers().AddNewtonsoftJson();
```

### Lifetimes

| Lifetime | Duration | Use Case |
|----------|----------|----------|
| Transient | Created each time | Stateless services |
| Scoped | Per HTTP request | DbContext, repositories |
| Singleton | Created once | Configuration, services |

**Repository uses Scoped** because DbContext should live for one request.

## Error Handling and Validation

### Model State Validation

```csharp
if (!ModelState.IsValid)
{
    return BadRequest(ModelState);
}
```

Automatically validates:
- Required fields (via `[Required]`)
- Field lengths (via `[MaxLength]`)
- Data types
- Custom validators

### Exception Handling

Current implementation:
- Returns appropriate HTTP status codes
- Logs warnings for not-found conditions
- Returns BadRequest for validation errors

Production considerations:
- Global exception handler middleware
- Logging to file/cloud service
- Error detail masking (don't expose stack traces)

## Logging

### Configuration

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

### Usage in Controllers

```csharp
private readonly ILogger<CitiesController> _logger;

public CitiesController(... ILogger<CitiesController> logger)
{
    _logger = logger;
}

public async Task<ActionResult<IEnumerable<CityInfoDto>>> GetCities()
{
    _logger.LogInformation("Getting all cities");
    // ...
}
```

### Log Levels

| Level | Usage |
|-------|-------|
| Trace | Very detailed debugging |
| Debug | Debugging information |
| Information | General flow information |
| Warning | Unexpected but handled situations |
| Error | Errors requiring attention |
| Critical | Critical failures |

## Entity Framework Core Integration

### DbContext Configuration

```csharp
public class CityInfoDbContext : DbContext
{
    public DbSet<CityInfo> Cities { get; set; }
    public DbSet<PointsOfInterest> PointsOfInterest { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Fluent API configuration
        modelBuilder.Entity<CityInfo>(entity =>
        {
            entity.HasKey(e => e.CityId);
            entity.Property(e => e.CityName).IsRequired().HasMaxLength(50);
        });
    }
}
```

### Relationships

**One-to-Many: CityInfo ? PointsOfInterest**

```csharp
// In CityInfo
public ICollection<PointsOfInterest> PointsOfInterest { get; set; }

// In PointsOfInterest
public CityInfo City { get; set; }
public int CityId { get; set; } // Foreign key

// In DbContext
.HasMany(e => e.PointsOfInterest)
.WithOne(p => p.City)
.HasForeignKey(p => p.CityId)
.OnDelete(DeleteBehavior.Cascade)
```

### Query Optimization

```csharp
// Eager loading for related data
public async Task<CityInfo?> GetCityWithPointsOfInterestAsync(int cityId)
{
    return await _context.Cities
        .Include(c => c.PointsOfInterest) // Eager load POIs
        .FirstOrDefaultAsync(c => c.CityId == cityId);
}

// Lazy loading would require separate query
// Projection for specific fields would reduce data transfer
```

## Testing Considerations

### Unit Testing the Repository

```csharp
[Test]
public async Task GetCityByIdAsync_WithValidId_ReturnsCityInfo()
{
    // Arrange
    var options = new DbContextOptionsBuilder<CityInfoDbContext>()
        .UseInMemoryDatabase("TestDb")
        .Options;
    
    using (var context = new CityInfoDbContext(options))
    {
        var city = new CityInfo { CityId = 1, CityName = "Test" };
        context.Cities.Add(city);
        await context.SaveChangesAsync();
    }

    // Act
    using (var context = new CityInfoDbContext(options))
    {
        var repository = new CityInfoRepository(context);
        var result = await repository.GetCityByIdAsync(1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Test", result.CityName);
    }
}
```

### Mocking in Controller Tests

```csharp
[Test]
public async Task GetCities_ReturnsOkWithCities()
{
    // Arrange
    var mockRepository = new Mock<ICityInfoRepository>();
    var mockMapper = new Mock<IMapper>();
    var mockLogger = new Mock<ILogger<CitiesController>>();
    
    var cities = new List<CityInfo> { new CityInfo { CityId = 1 } };
    mockRepository.Setup(r => r.GetAllCitiesAsync())
        .ReturnsAsync(cities);
    
    var controller = new CitiesController(mockRepository.Object, mockMapper.Object, mockLogger.Object);

    // Act
    var result = await controller.GetCities();

    // Assert
    Assert.IsInstanceOf<OkObjectResult>(result.Result);
}
```

## Security Recommendations

### Current Implementation
- Focuses on architecture demonstration
- No authentication/authorization

### Production Implementation

1. **Authentication**
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* config */ });

app.UseAuthentication();
```

2. **Authorization**
```csharp
[Authorize]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> DeleteCity(int cityId) { }
```

3. **Input Validation**
```csharp
[Required]
[StringLength(50, MinimumLength = 2)]
public string CityName { get; set; }
```

4. **Rate Limiting**
```csharp
builder.Services.AddRateLimiter(options => { /* config */ });
app.UseRateLimiter();
```

5. **HTTPS Only**
```csharp
app.UseHttpsRedirection();
```

## Performance Optimization

### Database Query Optimization

```csharp
// Include only needed data
public async Task<IEnumerable<CityInfo>> GetCitiesForListAsync()
{
    return await _context.Cities
        .Select(c => new CityInfo 
        { 
            CityId = c.CityId, 
            CityName = c.CityName 
        })
        .ToListAsync();
}
```

### Caching

```csharp
builder.Services.AddMemoryCache();

[HttpGet]
public async Task<ActionResult<IEnumerable<CityInfoDto>>> GetCities()
{
    if (!_cache.TryGetValue("allCities", out var cities))
    {
        cities = await _repository.GetAllCitiesAsync();
        _cache.Set("allCities", cities, TimeSpan.FromMinutes(10));
    }
    return Ok(cities);
}
```

### Pagination

```csharp
public async Task<IEnumerable<CityInfo>> GetCitiesAsync(int page = 1, int pageSize = 10)
{
    return await _context.Cities
        .OrderBy(c => c.CityName)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
}
```

## Deployment Considerations

### Configuration Management

```csharp
// Development
builder.Configuration.AddJsonFile("appsettings.json", optional: false);
builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true);

// Production
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddKeyVault(); // Azure Key Vault example
```

### Database Migrations

```bash
# Create migration
dotnet ef migrations add InitialCreate

# Apply migrations
dotnet ef database update

# In production (automatic)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CityInfoDbContext>();
    context.Database.Migrate();
}
```

### Monitoring and Logging

Production setup:
- Application Insights
- Serilog for structured logging
- Health checks
- Metrics collection

## Versioning Strategy

Future enhancement for API versioning:

```csharp
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/cities")]
public class CitiesController : ControllerBase { }
```

Allows:
- Multiple API versions running
- Gradual deprecation of old versions
- Backward compatibility

---

This implementation provides a solid foundation for building scalable, maintainable REST APIs with ASP.NET Core following industry best practices and SOLID principles.
