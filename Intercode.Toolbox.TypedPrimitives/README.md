# Intercode.Toolbox.TypedPrimitives

The project name is Intercode.Toolbox.TypedPrimitives.

## Description

This project provides a source generator called `TypedPrimitiveSourceGenerator` that generates code for readonly record structs based on a marker attribute. It is designed to generate code for typed primitives with various converters.

## Usage

To use the `TypedPrimitiveSourceGenerator`, follow these steps:

1. Add the `Intercode.Toolbox.TypedPrimitives` package to your project.

2. Create a marker attribute that will be used to identify the types for code generation. For example:
3. Apply the marker attribute to the readonly record structs that you want to generate code for. For example:
4. Build your project. The `TypedPrimitiveSourceGenerator` will automatically generate code for the marked types.

5. Use the generated code in your project. The generated code will be named `<TypeName>.g.cs` and will be located in the same directory as the original source file.

## Public Methods and Classes

### TypedPrimitiveSourceGenerator

The `TypedPrimitiveSourceGenerator` class is the main class responsible for generating code for readonly record structs based on the marker attribute.

#### Public Methods

- `void Initialize(IncrementalGeneratorInitializationContext context)`: Initializes the source generator and registers the code generation logic.

### GeneratorModel

The `GeneratorModel` class represents the model used by the `TypedPrimitiveSourceGenerator` to generate code for a specific readonly record struct.

#### Public Properties

- `Type PrimitiveType`: Gets the type of the readonly record struct.
- `string TypeName`: Gets the name of the readonly record struct.
- `string Namespace`: Gets the namespace of the readonly record struct.
- `TypedPrimitiveConverter Converters`: Gets the converters to be used for the readonly record struct.
- `string? StringComparison`: Gets the string comparison option for the readonly record struct.
- `bool HasTypeConverter`: Gets a value indicating whether the readonly record struct has a type converter.
- `bool HasSystemTextJsonConverter`: Gets a value indicating whether the readonly record struct has a System.Text.Json converter.
- `bool HasNewtonsoftJsonConverter`: Gets a value indicating whether the readonly record struct has a Newtonsoft.Json converter.
- `bool HasEfCoreConverter`: Gets a value indicating whether the readonly record struct has an EF Core converter.
- `bool HasConverter(TypedPrimitiveConverter converter)`: Checks if the readonly record struct has a specific converter.

### EquatableArray<T>

The `EquatableArray<T>` struct represents a cache-friendly, immutable, and equatable array.

#### Public Properties

- `int Length`: Gets the length of the array.

#### Public Methods

- `bool Equals(EquatableArray<T> other)`: Determines whether the current `EquatableArray<T>` is equal to another `EquatableArray<T>`.
- `override bool Equals(object? obj)`: Determines whether the current `EquatableArray<T>` is equal to another object.
- `override int GetHashCode()`: Returns the hash code for the current `EquatableArray<T>`.
- `IEnumerator<T> GetEnumerator()`: Returns an enumerator that iterates through the elements of the `EquatableArray<T>`.
- `ReadOnlySpan<T> AsSpan()`: Returns a read-only span that represents the entire `EquatableArray<T>`.

### Result<T>

The `Result<T>` class represents the result of an operation that can either succeed or fail, with an optional value of type `T`.

#### Public Properties

- `T? Value`: Gets the value of the result if the operation succeeded.

#### Public Methods

- `Result(T value)`: Initializes a new instance of the `Result<T>` class with a successful result and a value.
- `static Result<T> Ok(T value)`: Creates a successful `Result<T>` with a value.
- `static Result<T> Fail(DiagnosticInfo diagnosticInfo)`: Creates a failed `Result<T>` with a single diagnostic info.
- `static Result<T> Fail(EquatableArray<DiagnosticInfo>? errors = null)`: Creates a failed `Result<T>` with multiple diagnostic infos.
- `static Result<T> Fail(IEnumerable<DiagnosticInfo> errors)`: Creates a failed `Result<T>` with multiple diagnostic infos.

### DiagnosticInfo

The `DiagnosticInfo` record represents cache-friendly diagnostic information used to generate a `Diagnostic` instance.

#### Public Properties

- `DiagnosticDescriptor Descriptor`: Gets the descriptor of the diagnostic.
- `Location? Location`: Gets the location of the diagnostic.
- `string? MessageArg`: Gets the message argument of the diagnostic.
- `EquatableArray<StringPair> Properties`: Gets the properties of the diagnostic.

#### Public Methods

- `Diagnostic ToDiagnostic()`: Converts the `DiagnosticInfo` to a `Diagnostic` instance.

## References

- [Andrew Lock's Source Generator series](https://andrewlock.net/creating-a-source-generator-part-9-avoiding-performance-pitfalls-in-incremental-generators/#6-take-care-with-diagnostics): Provides insights into the diagnostics infrastructure used in the `TypedPrimitiveSourceGenerator`.
The Intercode Toolbox is a collection of libraries that I have developed over the years; recently, I started collecting them in this unified tool set.
Although I have tried to make them as general purpose as possible, they are still modeled to the particular needs of the projects they were created for; 
this means it might not be ideal for your needs. I plan to make them more generic and open to PR contributions.
