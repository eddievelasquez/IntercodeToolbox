# Intercode.Toolbox.UnitTesting

A trimmable, AOT-compatible .NET library that provides tools for unit testing.

---

## `DebugAssertListener`

The `DebugAssertListner` hooks into the the `TraceListner` collection used by the [Debug](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.debug) 
and [Trace](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.trace) classes and listens for [TraceListener.Fail](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.tracelistener.fail) calls. When a failure is triggered, the listener will throw a `DebugAssertException` with the message provided by the failure.
This allows for unit tests to ensure that a particular assertion was triggered.

### Usage

```csharp
using var listener = new DebugAssertListener();
var act = () => Debug.Assert( false, "This is a test" );

act.Should()
    .Throw<DebugAssertException>();

listener.GetTrace()
        .Should()
        .Be( $"This is a test{Environment.NewLine}" );
```

### Reference

#### Constructor

Create an instance of the `DebugAssertListener` class. This class implements the [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable) interface 
and should be used in a `using` statement to ensure that the listener is removed from the [Trace.Listeners](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.trace.listeners) collection when it is no longer needed.
```csharp
DebugAssertListener()
```

#### Methods

Returns all the trace messages that have been captured by the listener.
```csharp
string GetTrace()
```

---

## `DebugAssertException`

The `DebugAssertException` is thrown by the `DebugAssertListener` when a failure is triggered by failures emitted by the [Debug](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.debug) 
or [Trace](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.trace) classes.

### Constructors

Initializes a new instance of the `DebugAssertException` class with the specified error message
```csharp
DebugAssertException( 
  string? message )
```

Initializes a new instance of the `DebugAssertException` class with the specified error and detail messages.
  ```csharp
public DebugAssertException(
   string? message,
   string? detailMessage )  
```
### Properties

Gets the detailed error message if it was provided; otherwise, it returns `null`.
```csharp
string? DetailMessage { get; }
```

---

## `DependencyInjection`

The `DependencyInjection` class provides foundational tools for unit testing classes that use the [Microsoft.Extensions.DependencyInjection](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection) library;
simplifying the process of creating and configuring a service provider for testing.

### Usage

```csharp
var values = new Dictionary<string, string?>
{
  { "MySection:Key1", "Value1" },
  { "MySection:Key2", "Value2" }
};

var configuration = DependencyInjection.CreateConfiguration( values );
var serviceProvider = DependencyInjection.CreateServiceProvider( 
  configuration,
  services => { services.AddSingleton( TimeProvider.System ); }
);

...

var timeProvider = serviceProvider.GetRequiredService<TimeProvider>();
var config = serviceProvider.GetRequiredService<IConfiguration>();
var section = config.GetSection( "MySection" );
var value = section["Key1"];

// Use configuration and timeProvider...
```

### Reference

#### Methods

- `CreateConfiguration` Creates a new `IConfigurationRoot` instance with optionally specified values by a [Dictionary](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2) containing the configuration values
and/or an [Action](https://learn.microsoft.com/en-us/dotnet/api/system.action-1) that adds values to the configuration builder. If no value sare provided
by the dictionary or the action, the configuration will be empty.

```csharp
static IConfigurationRoot CreateConfiguration(
    Dictionary<string, string?>? values = null,
    Action<IConfigurationBuilder>? configureSource = null )
```

- `CreateServiceProvider` creates a new `IServiceProvider` instance with the specified configuration and an optional action that allows for services to be added to the service collection.
By default, the service collection will contain the configuration as a singleton instance.

```csharp
static IServiceProvider CreateServiceProvider(
    IConfigurationRoot configurationRoot,
    Action<IServiceCollection>? servicesSetter = null )
```

---

## License

This project is licensed under the [MIT License](LICENSE).
