# Intercode.Toolbox.UnitTesting.UnitTesting.XUnit

A trimmable, AOT-compatible .NET library that provides logging and dependency injection tools for unit testing using the [XUnit](https://xunit.net/) library.

---

## `XUnitLogger` class

The `XUnitLogger` class implements the [ILogger](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.ilogger) 
interface and logs messages to a [ITestOutputHelper](https://xunit.net/docs/capturing-output) test output.<br>
Even the `XUnitLogger` can be created directly as show below, it is recommended to use `XUnitLoggerProvider` to create instances 
of the logger; or better yet, use obtain the logger through Dependency Injection by using deriving from the `DependencyInversionTestBase`
abstract class.

### Usage

```csharp
// _outputHelper is an instance of ITestOutputHelper, passed in through the constructor of the test fixture class
var logger = new XUnitLogger( _outputHelper, "Test", LogLevel.Information );
logger.LogInformation( "This is a test" );
```

### Reference

- Initialize a new instance of the `XUnitLogger` class. The `categoryName` parameter is used to filter log messages and the 
  `logLevel` parameter is used to filter log messages by severity. If a category name is not provided, an empty string will be used.

```csharp
XUnitLogger(
    ITestOutputHelper output,
    string? categoryName = null,
    LogLevel logLevel = LogLevel.Error )
```
- `Log` writes a log entry to the XUnit test output.

```csharp
void Log<TState>(
    LogLevel logLevel,
    EventId eventId,
    TState state,
    Exception? exception,
    Func<TState, Exception?, string> formatter )
```

- `IsEnabled` checks if the given log level is enabled for logging.

```csharp
bool IsEnabled( LogLevel logLevel )
```

- `BeginScope` begins a logical operation scope. The return scope object must be disposed to end the scope.

```csharp
IDisposable BeginScope<TState>(
    TState state )
    where TState: notnull
```

---

## `XUnitLoggerProvider` class

The `XUnitLoggerProvider` class is used to create `XUnitLogger` instances and implements the [ILoggerProvider](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.iloggerprovider)
interface. The logger provider must be disposed when it's no longer needed.<br>
Even the `XUnitLoggerProvider` can be created directly as show below, it is recommended to obtain loggers through Dependency Injection by using deriving from the `DependencyInversionTestBase`
abstract class.

### Usage

```csharp
// _outputHelper is an instance of ITestOutputHelper, passed in through the constructor of the test fixture class
using var provider = new XUnitLoggerProvider( _output, LogLevel.Information );
var logger = provider.CreateLogger( "Test" );
logger.LogInformation( "This is a test" );
```

### Reference

- Create a new instance of the `XUnitLoggerProvider` class. The value for the `outputHelper` parameter is usually passed in to the test fixture's constructor.
  The `logLevel` parameter is used to filter log messages by severity.
```csharp
XUnitLoggerProvider(
    ITestOutputHelper outputHelper,
    LogLevel logLevel = LogLevel.Error )
```

- `CreateLogger` creates a new logger instance with the specified category name; it will return an existing logger if one with the same category name already exists.

```csharp
ILogger CreateLogger( string categoryName )
```

---

## `DependencyInversionTestBase` class

The `DependencyInversionTestBase` class is an abstract class that provides logging and dependency injection services for unit test fixtures.

### Usage

```csharp

public class MyTestFixture : DependencyInversionTestBase
{
    public MyTestFixture( ITestOutputHelper outputHelper )
      : base( outputHelper, typeof(ILogger<MyTestFixture>), AddServices )
    {
    }

    [Fact]
    public void MyTest()
    {
      var dbContext = ServiceProvider.GetRequiredService<TestDbContext>();

      // Perform test
    }

    private static void AddServices( IServiceCollection services )
    {
      services.AddDbContext<TestDbContext>(
        options =>
        {
          var connection = new SqliteConnection( "Data Source=:memory:" );
          connection.Open();
          options.UseSqlite( connection );
        }
      );
    }
}
```

### Reference

#### Constructor

Initialize a new instance of the `DependencyInversionTestBase` class. The `outputHelper` parameter is destination for log messages.
The `loggerType` parameter is used to specify the type of logger to use. The `addServices` parameter is a delegate that is used to configure the service provider.
The `configurationValues` and `configureSource` parameters are used to configure the configuration provider. 
See the `DependencyInjection.CreateConfiguration` method in the `Intercode.Toolbox.UnitTesting` package for more information.

```csharp
DependencyInversionTestBase(
    ITestOutputHelper outputHelper,
    Type loggerType,
    LogLevel logLevel = LogLevel.Information,
    Action<IServiceCollection>? servicesSetter = null,
    Dictionary<string, string?>? configurationValues = null,
    Action<IConfigurationBuilder>? configureSource = null )
```

#### Properties

- `ServiceProvider` gets the service provider that was created for the test fixture.

```csharp
IServiceProvider ServiceProvider { get; }
```

- `Logger` gets the logger that was created for the test fixture. The logger can also be obtained by calling `ServiceProvider.GetRequiredService<ILogger<T>>()`.

```csharp
ILogger Logger { get; }
```

---

## License

This project is licensed under the [MIT License](LICENSE).
