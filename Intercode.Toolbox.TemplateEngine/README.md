# Intercode.Toolbox.TemplateEngine

A fast and simple text templating engine.

## Updates
- **Version 2.4** - Added dynamic macro support.

## Table of Contents
<!--TOC-->
  - [Description](#description)
  - [Processing Macros in Templates: A Quick Guide](#processing-macros-in-templates-a-quick-guide)
  - [Reference](#reference)
    - [`TemplateEngineOptions` class](#templateengineoptions-class)
      - [Constructor](#constructor)
      - [Properties](#properties)
    - [`TemplateCompiler` class](#templatecompiler-class)
      - [Constructor](#constructor)
      - [Methods](#methods)
    - [`MacroValueGenerator` delegate](#macrovaluegenerator-delegate)
    - [`MacroProcessorBuilder` class](#macroprocessorbuilder-class)
      - [Constructor](#constructor)
      - [Methods](#methods)
      - [Extension Methods](#extension-methods)
    - [`MacroProcessor` class](#macroprocessor-class)
      - [Methods](#methods)
  - [Benchmarks](#benchmarks)
  - [License](#license)
<!--/TOC-->

## Description

A template is a string that can include one or more macros, which are placeholders replaced by their corresponding values. 
These values can be dynamically generated. By default, macro names are strings enclosed by `$` characters. For example:

```
Hello, $Name$! Today is $Now:yyyyMMdd$.
```

> **NOTE:** A delimiter can be any character, but it must be the same for the start and end of the macro name.
The delimiter is escaped by doubling it, so `$$` is a literal `$` character. To avoid excessive escaping and
simplify templates, choose a character not commonly found in the template's text. For C# code, the backtick `` ` `` 
character is another good delimiter choice.

The macro names are case-insensitive, meaning that `$Name$` and `$name$` will reference the same macro value.

## Processing Macros in Templates: A Quick Guide

Let's look at a simple example of how to process macros in a template:

```csharp
TemplateCompiler compiler = new();
Template template = compiler.Compile("Hello, $Name$! Today is $Now:yyyyMMdd$. You are $Age$ years old!");

MacroProcessorBuilder builder = new();
builder.AddStandardMacros()
       .AddMacro("Name", "John")
       .AddMacro("Age", _ => Random.Shared.Next(18, 100).ToString());

MacroProcessor processor = builder.Build();
StringWriter writer = new();
processor.ProcessMacros(template, writer);

var result = writer.ToString();

// Result should be "Hello, John! Today is 20241023. You are 27 years old!" 
// Assuming today is October 23, 2024 and the generated random number is 27.
```

## Step-by-Step Guide

### 1. Create a TemplateCompiler Instance

First, create a `TemplateCompiler` instance to parse the template text and generate a `Template` instance. 

Key points:
* A `Template` consists of segments that can be either constant text or macros.
* You can cache and reuse `Template` instances for different macro values.
* Use `TemplateEngineOptions` to customize macro delimiters and argument separators.

### 2. Build a MacroProcessor

Next, create a `MacroProcessor` using the `MacroProcessorBuilder`:

* Initialize a new `MacroProcessorBuilder`
* Use `AddMacro()` to add macros using either:
  * Static values: name-value pairs.
  * Dynamic values: functions that generate values.
* Optionally add standard macros using `AddStandardMacros()`
* Call `Build()` to create the `MacroProcessor`

**Performance Tip:** Creating a `MacroProcessor` is relatively computationally expensive. For high-performance scenarios 
(like Roslyn source generators), reuse the instance when processing multiple templates with the same **static** macro values.

### 3. Process Macros in the Template

Finally, process the template:

1. Call `ProcessMacros()` on your `MacroProcessor` instance
2. Pass the `Template` instance generated in Step 1 and a `StringWriter` instance.
3. Retrieve the final output from the `StringWriter`.

**Note:** If you used custom `TemplateEngineOptions` in Step 1, make sure to use the same options when creating the `MacroProcessorBuilder`.

## Custom Dynamic Macros

Creating a custom dynamic macro is easy: just call the `AddMacro` method and supply a `MacroValueGenerator` delegate to
generate the macro's value. The example below shows how to create a dynamic macro that returns the current date and 
time in a specified format:

```csharp
 MacroProcessorBuilder builder = new();
 builder.AddMacro( "Random", _ => Random.Shared.Next() );
```

You can customize your dynamic macro's behavior by using an argument, accessible through the `argument` parameter in the
`MacroValueGenerator` delegate. If the macro is instantiated without an argument, `argument` will be an empty span. Otherwise, 
you can convert it to a string and use it as needed. The argument value is fully transparent to the macro processor, with
the only limitation being that it cannot contain the macro delimiter character.

```csharp
 MacroProcessorBuilder builder = new();
 builder.AddMacro( "Random", arg => 
    {
      if( arg.IsEmpty ) 
      {
        return Random.Shared.Next().ToString();
      }
        
      return Random.Shared.Next( int.Parse( arg ) )
    });
```

In the example above, the `Random` macro generates a random non-negative number less than the specified value. If no argument is provided,
it generates a random non-negative number less than `int.MaxValue`.

With the following template:

```
Random number: $Random$. Random number less than 100: $Random:100$.
```

The first macro will generate a random number less than `int.MaxValue`, while the second macro will generate a random number less than 100.

> **NOTE**: Any exception thrown by a macro value generator will be caught and the macro's value will be set to the exception's
error message.

## Reference
---

### `TemplateEngineOptions` class

Represents the options for the classes in the template engine.

#### Constructor

```csharp
TemplateEngineOptions(
    char? macroDelimiter = null,
    char? argumentSeparator = null )
```

Creates a new instance of the `TemplateEngineOptions` class. The `macroDelimiter` parameter specifies the character 
used to delimit macro; if `null`, the value in the `DefaultMacroDelimiter` constant is used. 
The `argumentSeparator` parameter specifies the character used to separate the macro's name from it's argument; if `null`,
the value in the `DefaultArgumentSeparator` constant is used.

An `ArgumentException` exception will be thrown if either `macroDelimiter` or `argumentSeparator` are a non-punctuation character.


#### Properties

```csharp
char MacroDelimiter { get; }
```

`MacroDelimiter` gets the character used to delimit macro names in the template text.

```csharp
char ArgumentSeparator { get; }
```

`ArgumentSeparator` gets the character used to separate a macro's name from its argument in the template text.


### `TemplateCompiler` class

Compiles a template text into a `Template`

#### Constructor

```csharp
TemplateCompiler( TemplateEngineOptions? options = null )
```

Creates a new instance of the `TemplateCompiler` class. The `options` parameter specifies the option values used during compilation;
if `null`, the values in `TemplateEngineOptions.Default` are used.

#### Methods

```csharp
Template Compile( string text )
```

`Compile` compiles the specified template text into a `Template` instance; an `ArgumentException` is thrown if the template text is `null` or empty.

---

### `MacroValueGenerator` delegate

```csharp
delegate string MacroValueGenerator( ReadOnlySpan<char> argument );
```

Defines a method that will generate a dynamic macro's value. The `argument` parameter is the macro's argument, which is an optional text, separated by a
colon `:` after macro's name and the closing delimiter. If the template didn't specify an argument, the `argument` parameter will be `ReadOnlySpan<char>.Empty`.

---

### `MacroProcessorBuilder` class

#### Constructor

```csharp
MacroProcessorBuilder( TemplateEngineOptions? options = null )
```

Builds a `MacroProcessor` instance with the specified macros. The `options` parameter specifies the option values used while processing macros;
if `null`, the values in `TemplateEngineOptions.Default` are used.

#### Methods

```csharp
MacroProcessorBuilder AddMacro( string name, string value )
```

`AddMacro` adds a static value macro to the builder. The `name` parameter is the macro name, and the `value` parameter is the macro value. 
An `ArgumentException` is thrown if the macro name is `null`, empty, all whitespaces, or contains any character that is not alphanumeric.
or an underscore.

```csharp
MacroProcessorBuilder AddMacro( string name, MacroValueGenerator generator )
``` 

`AddMacro` adds a dynamic value macro to the builder. The `name` parameter is the macro name, and the `generator` parameter is a function 
that generates a string value when called.
An `ArgumentException` is thrown if the macro name is `null`, empty, all whitespaces, or contains any character that is not alphanumeric.

```csharp
MacroProcessor Build()
```

`Build` creates a `MacroProcessor` instance with the macros added to the builder. The builder instance can be reused to generate a new 
`MacroProcessor` instance if required.

#### Extension Methods

```csharp
static MacroProcessorBuilder AddStandardMacros( this MacroProcessorBuilder builder )
```

`AddStandardMacros` adds the following dynamic macros to the builder:

- `NOW` - Gets the current local date and time. The optional argument is the format string passed to the `DateTime.ToString(String)` method.
- `UTC_NOW` - Gets the current UTC date and time. The optional argument is the format string passed to the `DateTime.ToString(String)` method.
- `GUID` - Generates a new `Guid`. The optional argument is the format string passed to the `Guid.ToString(String)` method.
- `MACHINE` - Gets the name of the local computer as returned by the `Environment.MachineName` property.
- `OS` - Gets the name of the operating system as returned by the `Environment.OSVersion.VersionString` property.
- `USER` - Gets the name of the current user as returned by the `Environment.UserName` property.
- `CLR_VERSION` - Gets the version of the Common Language Runtime as returned by the `Environment.Version` property.
- `ENV` - Gets the value of the environment variable specified by the argument as returned by the `Environment.GetEnvironmentVariable(String)` method.

If any macro value generator throws an exception, the macro's value will be set to the exception's error message.

### `MacroProcessor` class

Processes macros in a `Template` instance and writes the result to a `TextWriter`.

#### Methods

```csharp
void ProcessMacros( Template template, TextWriter writer )
 ```

`ProcessMacros` replaces macros in the specified `Template` instance with their corresponding values and writes the result to the provided `TextWriter`.

 ```csharp
 string? GetMacroValue( string macroName )
 string? GetMacroValue( string macroName, ReadOnlySpan<char> argument )
 ```

`GetMacroValue` returns the value of the specified macro name; the macro name shouldn't include the delimiters. If the macro name
 is not found, `null` is returned. If the macro has an argument, it should be passed as the second parameter.

 
## Benchmarks
---

### Benchmark Setup

#### Template text

To benchmark the `TemplateEngine` we are going to parse the following template, taken from one of the standard templates
from the [Intercode.Toolbox.TypedPrimitives](https://www.nuget.org/packages/Intercode.Toolbox.TypedPrimitives/) package:
``` csharp
    // <auto-generated> This file has been auto generated by Intercode Toolbox Typed Primitives. </auto-generated>
    #nullable enable

    namespace $Namespace$;

    public partial class $TypeName$SystemTextJsonConverter: global::System.Text.Json.Serialization.JsonConverter<$TypeQualifiedName$>
    {
      public override bool CanConvert(
        global::System.Type typeToConvert )
      {
        return typeToConvert == typeof( $TypeQualifiedName$ );
      }

      public override $TypeQualifiedName$ Read(
        ref global::System.Text.Json.Utf8JsonReader reader,
        global::System.Type typeToConvert,
        global::System.Text.Json.JsonSerializerOptions options )
      {
        $TypeKeyword$? value = null;
        if( reader.TokenType != global::System.Text.Json.JsonTokenType.Null )
        {
          if( reader.TokenType == global::System.Text.Json.JsonTokenType.$JsonTokenType$ )
          {
            value = $JsonReader$;
          }
          else
          {
            bool converted = false;
            ConvertToPartial( ref reader, typeToConvert, options, ref value, ref converted );

            if ( !converted )
            {
              throw new global::System.Text.Json.JsonException( "Value must be a $JsonTokenType$" );
            }
          }
        }

        var result = $TypeQualifiedName$.Create( value );
        if( result.IsFailed )
        {
          throw new global::System.Text.Json.JsonException(
            global::System.Linq.Enumerable.First( result.Errors )
                  .Message
          );
        }

        return result.Value;
      }

      public override void Write(
        global::System.Text.Json.Utf8JsonWriter writer,
        $TypeQualifiedName$ value,
        global::System.Text.Json.JsonSerializerOptions options )
      {
        if ( value.IsDefault )
        {
          writer.WriteNullValue();
          return;
        }

        $JsonWriter$;
      }

      partial void ConvertToPartial(
        ref global::System.Text.Json.Utf8JsonReader reader,
        global::System.Type typeToConvert,
        global::System.Text.Json.JsonSerializerOptions options,
        ref $TypeKeyword$? value,
        ref bool converted );
    }
    
```

#### Macro values

And we are going to use the following macro values:

| Macro Name | Value |
|------------|-------|
| `Namespace` | `Benchmark.Tests` |
| `TypeName` | `TestType` |
| `TypeQualifiedName` | `Benchmark.Tests.TestType` |
| `TypeKeyword` | `string` |
| `JsonTokenType` | `String` |
| `JsonReader` | `reader.GetString()` |
| `JsonWriter` | `writer.WriteStringValue( value.Value )` |

#### Benchmark Code
Using [BenchmarkDotNet](https://benchmarkdotnet.org/)

```csharp
[MemoryDiagnoser]
public partial class MacroProcessingTests
{
  private readonly Template _template;
  private readonly MacroProcessor _macroProcessor;
  private readonly IReadOnlyDictionary<string, string> _macros;

  public MacroProcessingTests()
  {
    var helper = new TemplateEngineHelper();
    _template = helper.Compile();
    _macroProcessor = helper.CreateMacroProcessor();
    _macros = _macroProcessor.GetMacros();
  }

  [Benchmark( OperationsPerInvoke = 3 )]
  public void UsingMacroProcessor()
  {
    var writer = new StringWriter();
    _macroProcessor.ProcessMacros( _template, writer );
  }

  [Benchmark( Baseline = true, OperationsPerInvoke = 3 )]
  public void UsingStringBuilderReplace()
  {
    var sb = new StringBuilder( _template.Text );
    foreach( var (macro, value) in _macros )
    {
      sb.Replace( macro, value );
    }

    var processed = sb.ToString();
  }

  [Benchmark( OperationsPerInvoke = 3 )]
  public void UsingRegularExpressions()
  {
    var result = CreateMacroNameRegex()
      .Replace(
        _template.Text,
        match =>
        {
          var key = match.Groups[1].Value;
          return _macros.TryGetValue( key, out var value ) ? value : match.Value;
        }
      );
  }

  [GeneratedRegex( @"\$([^$]+)\$" )]
  private static partial Regex CreateMacroNameRegex();
}
```

> **NOTE**: The code for the `TemplateEngineHelper` class just compiles the text into a `Template` and creates a `MacroProcessor` instance.
It is kept in a separate class because we only want to measure the actual macro processing time in this benchmark.

#### Benchmark Results

As the results indicate, the `MacroProcessor` demonstrates significantly 
faster performance and lower memory allocation compared to the `StringBuilder` and `Regex` implementations.

![Results](https://raw.githubusercontent.com/eddievelasquez/IntercodeToolbox/refs/heads/main/Intercode.Toolbox.TemplateEngine/BenchmarkResults.png)

---

## License

This project is licensed under the [MIT License](LICENSE).
