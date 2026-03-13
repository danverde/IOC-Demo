# Teaching Guide: Interfaces and Inversion of Control (IoC)

## Overview

This project demonstrates the difference between tightly-coupled code (WithoutIOC) and loosely-coupled code using interfaces and dependency injection (WithIOC). Both projects implement the same weather forecast API, but with dramatically different approaches to testing and maintainability.

## Project Structure

```
LOS Demo/
├── WithoutIOC/          # Traditional approach with tight coupling
│   ├── Domain/
│   ├── Infrastructure/
│   ├── Application/
│   └── View/
├── WithoutIOC.Tests/    # 39 integration-style tests
├── WithIOC/             # Modern approach with interfaces & DI
│   ├── Domain/
│   ├── Infrastructure/  (includes interfaces)
│   ├── Application/     (includes interfaces)
│   └── View/
└── WithIOC.Tests/       # 38 unit tests with mocking
```

## Part 1: The Problem with Tight Coupling

### WithoutIOC Example

**WeatherService (WithoutIOC):**
```csharp
public class WeatherService
{
    private readonly string _connectionString;

    public WeatherService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<WeatherForecast>?> GetWeatherForecast(string zipCode)
    {
        // Creates concrete instances - TIGHT COUPLING
        var weatherStore = new WeatherStore(_connectionString);
        var weatherAdapter = new WeatherAdapter();

        bool isSupported = await weatherStore.IsZipCodeSupported(zipCode);
        // ... rest of implementation
    }
}
```

### Problems with This Approach

1. **Hard Dependencies**: `WeatherService` directly creates `WeatherStore` and `WeatherAdapter`
2. **Difficult to Test**: Cannot easily mock or replace dependencies
3. **Inflexible**: Cannot swap implementations without changing code
4. **Brittle**: Changes to dependencies require changes to service
5. **Integration Testing Only**: Tests must use real implementations

### Testing Impact (WithoutIOC)

**Test Example:**
```csharp
[Fact]
public async Task GetWeatherForecast_WithSupportedZipCode_ReturnsForecastList()
{
    // Must use real connection string
    var service = new WeatherService(TestConnectionString);
    var zipCode = "10001";

    // Calls REAL WeatherStore and REAL WeatherAdapter
    var result = await service.GetWeatherForecast(zipCode);

    Assert.NotNull(result);
    Assert.Equal(5, result.Count);
}
```

**Limitations:**
- Tests are slower (real delays from Task.Delay)
- Tests depend on WeatherStore and WeatherAdapter implementation
- Cannot test error scenarios easily
- Cannot verify that methods were called
- Tests break if dependencies break

## Part 2: The Solution - Interfaces and IoC

### WithIOC Example

**Step 1: Define Interfaces**
```csharp
public interface IWeatherStore
{
    Task<bool> IsZipCodeSupported(string zipCode);
}

public interface IWeatherAdapter
{
    Task<WeatherApiResponse> GetWeatherDataAsync(string zipCode);
}

public interface IWeatherService
{
    Task<List<WeatherForecast>?> GetWeatherForecast(string zipCode);
}
```

**Step 2: Depend on Abstractions**
```csharp
public class WeatherService : IWeatherService
{
    private readonly IWeatherStore _weatherStore;
    private readonly IWeatherAdapter _weatherAdapter;

    // Dependencies INJECTED via constructor
    public WeatherService(IWeatherStore weatherStore, IWeatherAdapter weatherAdapter)
    {
        _weatherStore = weatherStore ?? throw new ArgumentNullException(nameof(weatherStore));
        _weatherAdapter = weatherAdapter ?? throw new ArgumentNullException(nameof(weatherAdapter));
    }

    public async Task<List<WeatherForecast>?> GetWeatherForecast(string zipCode)
    {
        // Uses injected dependencies - LOOSE COUPLING
        bool isSupported = await _weatherStore.IsZipCodeSupported(zipCode);
        var apiResponse = await _weatherAdapter.GetWeatherDataAsync(zipCode);
        // ... rest of implementation
    }
}
```

**Step 3: Configure Dependency Injection**
```csharp
// Program.cs
builder.Services.AddScoped<IWeatherAdapter, WeatherAdapter>();
builder.Services.AddScoped<IWeatherStore>(sp => new WeatherStore(connectionString));
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<WeatherController>();
```

### Benefits of This Approach

1. **Loose Coupling**: Service depends on interfaces, not concrete classes
2. **Easy to Test**: Can mock dependencies
3. **Flexible**: Can swap implementations without changing service code
4. **Maintainable**: Changes to implementation don't affect consumers
5. **True Unit Testing**: Test components in isolation

## Part 3: Testing Comparison

### WithoutIOC Testing (Integration Style)

```csharp
// Creates REAL instances
var service = new WeatherService(TestConnectionString);

// Executes REAL logic in dependencies
var result = await service.GetWeatherForecast("10001");
```

**Characteristics:**
- ✅ Tests actual integration
- ❌ Slower execution
- ❌ Tests multiple units at once
- ❌ Cannot control dependency behavior
- ❌ Cannot verify interactions
- ❌ Difficult to test edge cases

### WithIOC Testing (Unit Style with Mocking)

```csharp
// Arrange: Create mocks
var mockStore = new Mock<IWeatherStore>();
var mockAdapter = new Mock<IWeatherAdapter>();

// Setup: Control behavior
mockStore.Setup(s => s.IsZipCodeSupported("10001")).ReturnsAsync(true);
mockAdapter.Setup(a => a.GetWeatherDataAsync("10001"))
    .ReturnsAsync(new WeatherApiResponse { /* ... */ });

// Create service with mocks
var service = new WeatherService(mockStore.Object, mockAdapter.Object);

// Act: Test only WeatherService logic
var result = await service.GetWeatherForecast("10001");

// Assert: Verify results
Assert.NotNull(result);

// Verify: Check interactions
mockStore.Verify(s => s.IsZipCodeSupported("10001"), Times.Once);
```

**Characteristics:**
- ✅ Fast execution (no real delays)
- ✅ Tests single unit in isolation
- ✅ Full control over dependency behavior
- ✅ Can verify method calls
- ✅ Easy to test edge cases
- ✅ Tests don't break when dependencies change

## Part 4: Real-World Testing Scenarios

### Scenario 1: Testing Error Handling

**WithoutIOC:** Hard to simulate errors
```csharp
// How do you make WeatherAdapter throw an exception?
// You'd need to modify the real class or use reflection
```

**WithIOC:** Easy with mocks
```csharp
mockAdapter.Setup(a => a.GetWeatherDataAsync(It.IsAny<string>()))
    .ThrowsAsync(new HttpRequestException("API unavailable"));

// Now test how service handles the exception
```

### Scenario 2: Testing That Methods Are Called

**WithoutIOC:** Impossible
```csharp
// Cannot verify that IsZipCodeSupported was called
// Can only check the final result
```

**WithIOC:** Built-in with Moq
```csharp
mockStore.Verify(s => s.IsZipCodeSupported("10001"), Times.Once);
mockAdapter.Verify(a => a.GetWeatherDataAsync(It.IsAny<string>()), Times.Never);
```

### Scenario 3: Testing Early Returns

**WithoutIOC:** Tests full flow
```csharp
[Fact]
public async Task GetWeatherForecast_WithUnsupportedZipCode_ReturnsNull()
{
    var service = new WeatherService(TestConnectionString);

    // WeatherAdapter.GetWeatherDataAsync is STILL called internally
    // even though it shouldn't be (code bug would go unnoticed)
    var result = await service.GetWeatherForecast("99999");

    Assert.Null(result);
}
```

**WithIOC:** Verifies exact behavior
```csharp
[Fact]
public async Task GetWeatherForecast_WithUnsupportedZipCode_ReturnsNull()
{
    mockStore.Setup(s => s.IsZipCodeSupported("99999")).ReturnsAsync(false);
    var service = new WeatherService(mockStore.Object, mockAdapter.Object);

    var result = await service.GetWeatherForecast("99999");

    Assert.Null(result);
    // VERIFY that adapter was NOT called (tests early return logic)
    mockAdapter.Verify(a => a.GetWeatherDataAsync(It.IsAny<string>()), Times.Never);
}
```

### Scenario 4: Testing Different API Responses

**WithoutIOC:** Limited control
```csharp
// WeatherAdapter returns hardcoded mock data
// Cannot easily test different temperature ranges, conditions, etc.
```

**WithIOC:** Complete control
```csharp
// Test with extreme temperatures
mockAdapter.Setup(a => a.GetWeatherDataAsync("10001"))
    .ReturnsAsync(new WeatherApiResponse
    {
        Temperatures = new[] { -40, -35, -30, -25, -20 },
        Conditions = new[] { "Freezing", "Freezing", "Freezing", "Freezing", "Freezing" }
    });

// Test with varying conditions
mockAdapter.Setup(a => a.GetWeatherDataAsync("90210"))
    .ReturnsAsync(new WeatherApiResponse
    {
        Temperatures = new[] { 35, 40, 45, 50, 55 },
        Conditions = new[] { "Hot", "Very Hot", "Scorching", "Extreme", "Dangerous" }
    });
```

## Part 5: Key Principles Demonstrated

### SOLID Principles

1. **Single Responsibility**: Each class has one job
   - `WeatherStore`: Database access
   - `WeatherAdapter`: External API calls
   - `WeatherService`: Business logic orchestration

2. **Open/Closed**: Open for extension, closed for modification
   - Can create new implementations of interfaces without changing existing code

3. **Liskov Substitution**: Can substitute implementations
   - Any `IWeatherAdapter` can be used where interface is expected

4. **Interface Segregation**: Small, focused interfaces
   - Each interface has only the methods needed

5. **Dependency Inversion**: Depend on abstractions
   - `WeatherService` depends on `IWeatherStore`, not `WeatherStore`

### Dependency Injection Benefits

1. **Testability**: Easy to mock dependencies
2. **Flexibility**: Swap implementations at runtime
3. **Lifetime Management**: Framework manages object lifecycles
4. **Configuration**: Centralized in one place (Program.cs)

## Part 6: Hands-On Exercises

### Exercise 1: Add a New Requirement

**Task**: Add a logging feature that logs every weather request.

**WithoutIOC Approach:**
- Modify `WeatherService` to create a logger instance
- Tightly couples service to specific logger
- Tests now depend on logging behavior

**WithIOC Approach:**
- Create `ILogger` interface
- Inject into `WeatherService`
- Mock in tests - logging doesn't affect test logic
- Can swap logging implementations without changing service

### Exercise 2: Change Data Source

**Task**: Switch from mock database to actual SQL Server.

**WithoutIOC Approach:**
- Modify `WeatherStore` class
- Hope you didn't break anything
- Re-run all tests

**WithIOC Approach:**
- Create `SqlWeatherStore : IWeatherStore`
- Update DI registration in Program.cs
- Service code unchanged
- Tests unchanged (still mock)

### Exercise 3: Add Caching

**Task**: Add caching layer to reduce API calls.

**WithoutIOC Approach:**
- Modify `WeatherAdapter` or `WeatherService`
- Complex to test caching logic
- Mixes concerns

**WithIOC Approach:**
- Create `CachedWeatherAdapter : IWeatherAdapter`
- Decorator pattern with dependency injection
- Test cache separately
- Swap in DI configuration

## Part 7: Test Metrics Comparison

### WithoutIOC.Tests
- **Tests**: 39
- **Test Type**: Integration/hybrid
- **Mocking**: None (except for BadRequest type checks)
- **Execution Time**: ~716ms
- **Dependencies**: Tests run against real implementations
- **Test Isolation**: Low (tests interact with multiple components)

### WithIOC.Tests
- **Tests**: 38
- **Test Type**: True unit tests
- **Mocking**: Moq framework
- **Execution Time**: ~719ms (similar, but more comprehensive)
- **Dependencies**: Fully mocked
- **Test Isolation**: High (each test is independent)

### Key Differences in Tests

| Aspect | WithoutIOC | WithIOC |
|--------|------------|---------|
| Setup Complexity | Simple (new instance) | Medium (mock setup) |
| Test Precision | Low (tests multiple units) | High (tests one unit) |
| Failure Diagnosis | Hard (which component failed?) | Easy (only one component tested) |
| Edge Case Testing | Limited | Comprehensive |
| Interaction Verification | Impossible | Built-in |
| Refactoring Safety | Medium | High |

## Part 8: When to Use Each Approach

### Use WithoutIOC Style When:
- Building simple prototypes
- Small applications with few dependencies
- Learning basic concepts
- Dependencies are stable and simple

### Use WithIOC Style When:
- Building production applications
- Need comprehensive test coverage
- Working in teams
- Application will evolve over time
- Dependencies are complex or external
- **Always - this is industry best practice**

## Part 9: Common Questions

### Q: Isn't IoC more complex?
**A**: Initially, yes. But complexity is front-loaded. Long-term maintenance and testing is much simpler.

### Q: Do I really need interfaces everywhere?
**A**: For dependencies, yes. For DTOs and domain models (like `WeatherForecast`), usually not.

### Q: What about performance?
**A**: Negligible. Modern DI containers are highly optimized. Testing speed improvements far outweigh any minor runtime overhead.

### Q: When should I write integration tests?
**A**: Always have both! Unit tests for fast feedback, integration tests for confidence. But unit tests should be the majority.

### Q: Can I mix both approaches?
**A**: Not recommended. Pick one consistently. WithIOC is the modern standard.

## Part 10: Next Steps

### To Practice:
1. Run both test suites and compare execution
2. Try adding a new feature to both projects
3. Try changing an implementation in both projects
4. Write a new test in both styles

### To Learn More:
- SOLID principles in depth
- Dependency Injection containers (Microsoft.Extensions.DependencyInjection)
- Mocking frameworks (Moq, NSubstitute)
- Test-Driven Development (TDD)
- Domain-Driven Design (DDD)

## Conclusion

The WithIOC project demonstrates that while interfaces and dependency injection require more upfront structure, they provide:

- **Better testability** through mocking
- **More flexibility** through abstraction
- **Easier maintenance** through loose coupling
- **Higher quality** through comprehensive unit tests

These benefits compound over time, making IoC essential for professional software development.

---

## Quick Reference

### Creating an Interface
```csharp
public interface IMyService
{
    Task<Result> DoSomething(string input);
}
```

### Implementing the Interface
```csharp
public class MyService : IMyService
{
    private readonly IDependency _dependency;

    public MyService(IDependency dependency)
    {
        _dependency = dependency;
    }

    public async Task<Result> DoSomething(string input)
    {
        return await _dependency.Process(input);
    }
}
```

### Registering with DI
```csharp
builder.Services.AddScoped<IDependency, Dependency>();
builder.Services.AddScoped<IMyService, MyService>();
```

### Testing with Mocks
```csharp
var mockDependency = new Mock<IDependency>();
mockDependency.Setup(d => d.Process("input")).ReturnsAsync(expectedResult);

var service = new MyService(mockDependency.Object);
var result = await service.DoSomething("input");

Assert.Equal(expectedResult, result);
mockDependency.Verify(d => d.Process("input"), Times.Once);
```
