# Intercode.Toolbox.TypedPrimitives

## Updates

- **Version 2.3.1** - Moved the template processing engine to its own package: [Intercode.Toolbox.TemplateEngine](https://www.nuget.org/packages/Intercode.Toolbox.TemplateEngine/).

## Description

[Primitive obsession](https://www.freecodecamp.org/news/what-is-primitive-obsession/) is a pervasive code smell in object-oriented programming that occurs when primitive types are used to represent domain concepts instead of creating custom types. This can lead to a variety of problems, including a lack of type safety, duplication of logic, and difficulty in understanding the code.
**TypedPrimitives** is an attempt to solve this problem in C# by providing an easy way to encapsulate primitives in a type-safe way.

There are several [projects](#references) that also attempt to tackle this problem, but most of them are focused on providing a way encapsulate object identifiers, 
hence not supporting other primitive types. **TypedPrimitives** is focused on providing a way to encapsulate most commonly-used primitive type, including `string`, 
`int`, `long`, `Guid`, `DateTime`, and `DateTimeOffset`. Support for other types can be added in the future.

Additionally, **TypedPrimitives** adds multiple features to the generated code, including:.
- Debugger display.
- `IComparable<T>`, `IComparable`, `IFormattable` and equality implementations.
- Casting to and from the primitive type.
- Hooks for adding custom validation and normalization of primitive values.
- Optional generation of JSON converters for `System.Text.Json` and `Newtonsoft.Json`.
- Optional generation of Entity Framework Core value converters.
- Optional generation of type converters.

A source generator automatically write the necessary code to encapsulate the primitive type by adding an attribute to a readonly struct.

## Usage

To use the **TypedPrimitives** source generator, follow these steps:

1. Add the `Intercode.Toolbox.TypedPrimitives` package to your project.
2. Create a `readonly partial struct` and add the `TypedPrimitive`, which identifies the primitive type that will be encapsulated and the optional converters to generate.
3. Optionally, add partial methods for validation and normalization of the primitive value.
4. Optionally add partial methods for custom serialization and deserialization of the primitive value.
4. Build your project; **TypedPrimities** will automatically generate code for the marked types.
5. Use the generated code in your project.

### Example

The following example shows how to create a `ZipCode` type that encapsulates a `string` primitive.

#### Creation
```csharp
using Intercode.Toolbox.TypedPrimitives;

[TypedPrimitive<string>] // Or [TypedPrimitive( typeof(string) )]
public readonly partial struct ZipCode;
```

How do we create an instance of the `ZipCode` type? By calling its `Create` method or explicitly casting a string value to the `ZipCode` type:

```csharp
Result<ZipCode> result = ZipCode.Create( "12345" );
ZipCode zipCodeB = (ZipCode) "12345";
```

By having a stronly-typed `ZipCode` type, instead of a string value, we can avoid one of the bigest problems with primitive obsession: passing the wrong value of the
same type to a method; this is an error the compiler will not detect and can be hard to track down.

#### Validation
However, the `ZipCode` type is not very useful without adding some validation logic. The following example shows how to add a partial method to validate when a `ZipCode` instance is beinmg created.

```csharp
[TypedPrimitive<string>]
public readonly partial struct ZipCode
{
  static partial void ValidatePartial(
    string? value,
    ref Result result )
  {
    result = Result.FailIf( /* validate zipcode format using regex */, "Invalid zipcode" );
  }
}
```
> **NOTE**: The `Create` uses the `Result` class from the [FluentValidation](https://www.nuget.org/packages/FluentValidation) 
> package to indicate whether operation succeeded or failed. Typed primitives that don't implement a validation method will 
> always succeed.
> Returning a `Result` instead of the typed primitive was done to avoid throwing exceptions for validation errors, 
> which can be expensive and can lead to performance issues.
> However; the casting operator will return the type primitive directly and throw an `InvalidOperationException` if
> provided with an invalid value.


To ensure the `ZipCode` was created with a valid value, just check the `IsSuccess` or `IsFailure` properties of the returned result; the actual `ZipCode` instance will be stored in the `Value` property of the result.
See the **FluentResults** library [documentation](https://github.com/altmann/FluentResults?tab=readme-ov-file#processing-a-result) for more details.

```csharp
var result = ZipCode.Create( "12345" );
if( result.IsSuccess )
{
  var zipCode = result.Value;
}

result = ZipCode.Create( "123XY" );
if ( result.IsFailure )
{
  Console.WriteLine( "Invalid zipcode" );
})
```

The validation code can be invoked directly by calling the `Validate` method, which returns a `Result` instance; or calling the 
`IsValid` method, which returns a boolean value indicating whether the value is valid.

```csharp
var result = ZipCode.Validate( "12345" );
if( result.IsSuccess )
{
  Console.WriteLine( "Valid zipcode" );
}

if( ZipCode.IsValid( "12345" ) )
{
  Console.WriteLine( "Valid zipcode" );
}
```
> **NOTE**: Since validation can be very complex, it might be possible that a single value may result in multiple errors, in this case,
> the use of the `Validate` method is recommended to get access to all the errors.

#### Normalization
Assume that your validation code allows for leading or trailing whitespaces in the value but you want to normalize the `ZipCode` value by trimming any whitespace:

```csharp
[TypedPrimitive<string>]
public readonly partial struct ZipCode
{
  // Validation code goes here...

  static partial void NormalizePartial(
    ref string value )
  {
    value = value.Trim();
  }
}
```

### Converters
**TypedPrimitives** can generate converters by assigning one or more `TypedPrimitiveConverter` values to the `Converters` property of the `TypedPrimitiveAttribute` attribute.
Currently the following values are supported:
- `None`: No converter will be generated. (This is the default value).
- `TypeConverter`: A custom `System.ComponentModel.TypeConverter` will be generated.
- `SystemTextJson`: A custom `System.Text.Json.Serialization.JsonConverter` will be generated.
- `NewtonsoftJson`: A custom `Newtonsoft.Json.JsonConverter` will be generated.
- `EFCoreValueConverter`: A custom `Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter` will be generated.

#### Type converter

A `System.ComponentModel.TypeConverter` provides a unified way of converting types of values to other types using the standard `TypeDescriptor.GetConverter` method.
See the [TypeConverter](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.typeconverter) class for more details.

Using the `ZipCode` class from the previous example, the following code shows how to generate a type converter for the `ZipCode` type:

```csharp
[TypedPrimitive<string>(Converters = TypedPrimitiveConverter.TypeConverter)]
public readonly partial struct ZipCode
{
  // Validation and/or normalization code goes here...
}
```

**TypePrimitives** will generate a `ZipCodeTypeConverter` class that inherits from `System.ComponentModel.TypeConverter` and can be used to convert `ZipCode` instances to and from `string`.

```csharp
var converter = TypeDescriptor.GetConverter( typeof( ZipCode ) );
var zipCode = (ZipCode) converter.ConvertFromString( "12345" );
Console.WriteLine( converter.ConvertToString( zipCode ) );
```

The generated type converter can be extended to provide custom conversion logic by providing implementations for the following partial methods (where `T` is the primitive type):
```csharp
partial void CanConvertFromPartial(
  System.ComponentModel.ITypeDescriptorContext? context,
  System.Type sourceType,
  ref bool canConvert );

partial void CanConvertToPartial(
  System.ComponentModel.ITypeDescriptorContext? context,
  System.Type? destinationType,
  ref bool canConvert );

partial void ConvertFromPartial(
  System.ComponentModel.ITypeDescriptorContext? context,
  System.Globalization.CultureInfo? culture,
  object value,
  ref T? convertedValue,
  ref bool converted );

partial void ConvertToPartial(
  System.ComponentModel.ITypeDescriptorContext? context,
  System.Globalization.CultureInfo? culture,
  T? value,
  System.Type destinationType,
  ref object? convertedValue,
  ref bool converted );
```

#### System.Text.Json converter

A `System.Text.Json.Serialization.JsonConverter<T>` provides a way to serialize and deserialize custom types using the `System.Text.Json` library.
See [JsonConverter](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to) for details.

Using the `ZipCode` class from the previous example, the following code shows how to generate a JsonConverter converter for the `ZipCode` type:

```csharp
[TypedPrimitive<string>(Converters = TypedPrimitiveConverter.SystemTextJson)]
public readonly partial struct ZipCode
{
  // Validation and/or normalization code goes here...
}
```

**TypePrimitives** will generate a `ZipCodeSystemTextJsonConverter` class that inherits from `System.Text.Json.Serialization.JsonConverter<ZipCode>` 
and can be used to convert `ZipCode` json properties to and from `string`. Deserializing invalid values will throw a `System.Text.Json.JsonException` exception.

```csharp
    public class Test
    {
      public ZipCode ZipCode { get; set; }
    }

    var json = """{"ZipCode":"12345"}""";
    var test = JsonSerializer.Deserialize<Test>( json );
    Console.WriteLine( test.ZipCode );  // Should print "12345"
```

The generated JSON converter can be extended to provide custom conversion logic by providing implementations for the following partial method (where `T` is the primitive type):
```csharp
partial void ConvertToPartial(
  ref System.Text.Json.Utf8JsonReader reader,
  System.Type typeToConvert,
  System.Text.Json.JsonSerializerOptions options,
  ref T? value,
  ref bool converted );
```

#### Newtonsoft.Json converter

A `Newtonsoft.Json.JsonConverter` provides a way to serialize and deserialize custom types using the `Newtonsoft.Json` library.
See [JsonConverter](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonConverter.htm) for details.

Using the `ZipCode` class from the previous example, the following code shows how to generate a JsonConverter converter for the `ZipCode` type:

```csharp
[TypedPrimitive<string>(Converters = TypedPrimitiveConverter.NewtonsoftJson)]
public readonly partial struct ZipCode
{
  // Validation and/or normalization code goes here...
}
```

**TypePrimitives** will generate a `ZipCodeNewtonsoftJsonConverter` class that inherits from `Newtonsoft.Json.JsonConverter` 
and can be used to convert `ZipCode` json properties to and from `string`. Deserializing invalid values will throw a `Newtonsoft.Json.JsonSerializationException` exception.

```csharp
    public class Test
    {
      public ZipCode ZipCode { get; set; }
    }

    var json = """{"ZipCode":"12345"}""";
    var result = JsonConvert.DeserializeObject<Test>( json );
    Console.WriteLine( test.ZipCode );  // Should print "12345"
```

The generated JSON converter can be extended to provide custom conversion logic by providing implementations for the following partial method (where `T` is the primitive type):
```csharp
partial void ConvertToPartial(
  ref Newtonsoft.Json.JsonReader reader,
  System.Type typeToConvert,
  ref T? convertedValue,
  ref bool converted );
```

#### EF Core value converter

A `Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter` provides a way to convert values to and from the database using the Entity Framework Core library.
See [Value Conversions](https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions) for details.

Using the `ZipCode` class from the previous example, the following code shows how to generate a value converter for the `ZipCode` type:

```csharp
[TypedPrimitive<string>(Converters = TypedPrimitiveConverter.EfCoreValueConverter)]
public readonly partial struct ZipCode
{
  // Validation and/or normalization code goes here...
}
```

**TypePrimitives** will generate a `ZipCodeEFCoreValueConverter` class that inherits from `Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<ZipCode, string>` 
and can be used to convert `ZipCode` properties to and from `string`. Deserializing invalid values will throw a `InvalidOperationException` exception.

```csharp
    public class TestEntity
    {
      public int Id { get; set; }
      public ZipCode ZipCode { get; set; }
    }

    dbContext.TestEntities.Add( new TestEntity { ZipCode = "12345" } );
    dbContext.SaveChanges();

    // 

    public class TestDbContext : DbContext
    {
      public DbSet<TestEntity> TestEntities { get; set; }

      protected override void OnModelCreating( ModelBuilder modelBuilder )
      {
        modelBuilder.Entity<TestEntity>()
          .Property( e => e.ZipCode )
          .HasConversion( new ZipCodeEFCoreValueConverter() );
      }
    }
```

## References {#references}

- [Dealing with primitive obsession](https://lostechies.com/jimmybogard/2007/12/03/dealing-with-primitive-obsession/) - Jimmy Bogard.
- [Roslyn Source Generators](https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.md) - Microsoft.
- [Creating a source generator](https://andrewlock.net/series/creating-a-source-generator/) - Andrew Lock.
- [StronglyTypedId](https://www.nuget.org/packages/StronglyTypedId/) - Andrew Lock


## License

This project is licensed under the [MIT License](LICENSE).
