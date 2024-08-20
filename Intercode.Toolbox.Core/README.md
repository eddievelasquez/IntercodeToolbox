# Intercode.Toolbox.Core

A trimmable, AOT-compatible .NET library that provides general purpose functionality created for several projects over the years.

---

# Updates

- **Version 2.1.6** - Added the `PathBuilder` class for building file and directory paths using a fluent interface.

---

## `AssemblyExtensions` 

### Methods

Provides extension methods for working with assemblies.

- `GetVersionString`: Gets the version string of the assembly; will return the assembly's informational version if the `AssemblyInformationalVersionAttribute` is found.
  ```csharp
      var version = Assembly.GetExecutingAssembly().GetVersionString();
      Console.WriteLine( version );
  ```

---

## `BaseConverter`

### Methods

Provides methods for converting numbers between different number bases. It supports bases from *2* to *62*, unlike the built-in `System.Convert.ToString` method, 
which only supports the *2*, *8*, *10* or *16* bases.

Base 62 is useful for generating shorter identifiers that are URL-safe and case-insensitive. It uses the characters `0-9`, `a-z`, and `A-Z`.

- `ConvertToString<T>` Converts the specified value to a string representation in the given radix.
  ```csharp
      var value = 1234567890;
      var base62 = BaseConverter.ConvertToString( value, 62 );
      Console.WriteLine( base62 );
  ```

- `ConvertFromString<T>` Converts the specified string representation in the given radix to a value.
  ```csharp
      var base62 = "2lkCB1";
      var value = BaseConverter.ConvertFromString<int>( base62, 62 );
      Console.WriteLine( value );
  ```

- `TryConvertFromString<T>` Tries to convert the specified string representation in the given radix to a value.
  ```csharp
      var base62 = "2lkCB1";
      if( BaseConverter.TryConvertFromString<int>( base62, 62, out var value ) )
      {
          Console.WriteLine( value );
      }
  ```

---

## `Disposer`

`Disposer` will invoke a user-provided action when disposed.

### Usage

  ```csharp
      using( new Disposer( static () => Console.WriteLine( "Disposed" ) ) )
      {
          Console.WriteLine( "Using Disposer" );
      }
  ```

 Example output:
 ```
 Using Disposer
 Disposed
 ```

 ---

## `EmailAddressListAttribute`

Represents a custom validation attribute that validates a collection of email addresses. The email validation is performed using 
the [EmailAddressAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.emailaddressattribute) class.

### Usage

```csharp
    public class MyModel
    {
      [EmailAddressList]
      public IEnumerable<string> EmailAddresses { get; set; } = new List<string>();
    }
```

---
## `EmbeddedResource`

Provides utility methods for loading embedded resources.

### Methods

- `LoadBytesFromResource` Loads the content of an embedded resource as a byte array.
```csharp
    var imageBytes = EmbeddedResource.LoadBytesFromResource( "logo.png" );
```

-`LoadFromResource` Loads the content of an embedded resource as a stream along with its content type.
```csharp
    var (stream, contentType) = EmbeddedResource.LoadFromResource( "logo.png" );
    Console.WriteLine( contentType );
```

---

## `IntegerEncoder`

Uses multiplicative inverses to obfuscate identifiers and avoid exposing sequential values that could potentially be exploited.
The algorithm is not cryptographically strong, but is very fast and cheap. It is suitable for obfuscating identifiers in URLs and similar scenarios; additional
encoding with the ```BaseConverter``` using base 62 can be used for further obfuscation.

The range of values that can be encoded is between -1,000,000,000 and -1,000,000,000.

### Methods

- `Encode` Encodes the specified integer value.
```csharp
    var encoded = IntegerEncoder.Encode( 42 );
    Console.WriteLine( encoded ); // 543321076
```

- `Decode` Decodes the specified encoded value.
```csharp
    var decoded = IntegerEncoder.Decode( 543321076 );
    Console.WriteLine( decoded ); // 42
```

---

## `Mime`

Provides methods for working with MIME types. The list of mappings can be extended by adding new entries to the `Mime.Mappings` dictionary.

### Methods

- `GetContentType` Gets the content type based on the file name.

```csharp
    var contentType = Mime.GetContentType( "sample.txt" );
    Console.WriteLine( contentType ); // text/plain
```

---

## `ObjectExtensions`

Provides extension methods for the `System.Object` class.

### Methods

- `IsNumber` Determines whether the specified object is a number.

```csharp
    object obj = 42;
    Console.WriteLine( obj.IsNumber() ); // True

    obj = "Hello, World!";
    Console.WriteLine( obj.IsNumber() ); // False
```

---

## `PathBuilder`

Provides a fluent interface for building file and directory paths.

### Usage

```csharp
    var path = new PathBuilder(@"c:\temp\myfile.tmp")
        .ChangeFilename( filename => $"{DateTime.Now:yyMMdd}-{filename}" )
        .Build();

    Console.WriteLine( path ); // c:\temp\240820-myfile.tmp
```

### Methods

- `SetDirectory` sets or replaces the path's directory; if `null` is provided, the directory is removed.
- `ChangeDirectory` changes the path's directory using a transformation function, which receives the path's current directory.
- `AppendDirectory` appends a directory to the path.
- `AppendDirectories` appends one or more directories to the path.
- `SetFilename` sets or replaces the path's filename; if `null` is provided, the filename is removed.
- `ChangeFilename` changes the path's filename using a transformation function, which receives the path's current filename.
- `SetExtension` sets or replaces the path's extension; if `null` is provided, the extension is removed.
- `ChangeExtension` changes the path's extension using a transformation function, which receives the path's current extension.
- `Build` builds the final path string.

---

## `PhoneListAttribute`

Represents a custom validation attribute that validates a list of phone numbers; the validation is performed using the [PhoneAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.phoneattribute) class.

### Usage

```csharp
    public class MyModel
    {
      [PhoneList]
      public IEnumerable<string> PhoneNumbers { get; set; } = new List<string>();
    }
```

---

## `ReaderWriterLockExtensions`

Extension methods for the `System.Threading.ReaderWriterLockSlim` class.

### Methods

- `ReadLock` Acquires a read lock on the specified `ReaderWriterLockSlim` object. The lock is released when the returned `IDisposable` object is disposed.

```csharp
    using( readerWriterLock.ReadLock() )
    {
        // Read operation
    }
```

- `WriteLock` Acquires a write lock on the specified `ReaderWriterLockSlim` object. The lock is released when the returned `IDisposable` object is disposed.

```csharp
    using( readerWriterLock.WriteLock() )
    {
        // Write operation
    }
```

- `UpgradeableReadLock` Acquires an upgradeable read lock on the specified `ReaderWriterLockSlim` object. The lock is released when the returned `IDisposable` object is disposed.

```csharp
    using( readerWriterLock.UpgradeableReadLock() )
    {
        // Upgradeable read operation

        using ( readerWriterLock.WriteLock() )
        {
            // Write operation
        }

        // Upgradeable read operation
    }
```

---

## `StreamExtensions`

Provides extension methods to read and write values to and from a `Sytem.Stream` object.

### Methods

- `ToByteArray` Reads the entire stream and returns its content as a byte array. It has an optimization for `MemoryStream` where it behaves exactly as calling `ToArray()`.

```csharp
    using( var stream = new MemoryStream( new byte[] { 1, 2, 3, 4, 5 } ) )
    {
        var bytes = stream.ToByteArray();
        Console.WriteLine( string.Join( ", ", bytes ) ); // 1, 2, 3, 4, 5
    }
```

- `ToByteArrayAsync` Asynchronously converts a stream to a byte array

```csharp
    await using( var stream = new MemoryStream( new byte[] { 1, 2, 3, 4, 5 } ) )
    {
        var bytes = await stream.ToByteArrayAsync();
        Console.WriteLine( string.Join( ", ", bytes ) ); // 1, 2, 3, 4, 5
    }
```

---

## `StringExtensions`


Provides extension methods for `System.String` objects.

### Methods

- `RemoveDiacritics` Removes diacritics from the specified string.

```csharp
    var text = "Héllö, Wörld!";
    var normalized = text.RemoveDiacritics();
    Console.WriteLine( normalized ); // Hello, World!
```

---

## `UriBuilderExtensions`


Provides extension methods for `System.UriBuilder` instances.

- `AppendPath` Appends a path segment to the URI builder's path; ensures that the appended path value is URL encoded.

```csharp
    var builder = new UriBuilder( "https://example.com/" );
    builder.AppendPath( "<api>" );
    Console.WriteLine( builder.Uri ); // https://example.com/%3Capi%3E
```

- `AppendQuery` Appends a query parameter to the URI builder's query; ensures that the appended query key is URL encoded.

```csharp
    var builder = new UriBuilder( "https://example.com/" );
    builder.AppendQuery( "<key>" );
    Console.WriteLine( builder.Uri ); // https://example.com/?%3Ckey%3E
```

- `AppendQuery` Appends a query parameter with a value to the URI builder's query; ensures that the appended query key and value are URL encoded.

```csharp
    var builder = new UriBuilder( "https://example.com/" );
    builder.AppendQuery( "<key>", "<value>" );
    Console.WriteLine( builder.Uri ); // https://example.com/?%3Ckey%3E=%3Cvalue%3E
```

---

## `VariableIntegerEncoder`

Provides methods for encoding and decoding integers using the Variable Length Quantity (VLQ) encoding scheme. 
This method is used in several protocols to encode integers in a compact form, such as MIDI and Google Protocol Buffers.
Supports encoding `sbyte`, `byte`, `short`, `ushort`, and `int` values. The maximum required buffer size is 5 bytes.

- `TryEncode` Tries to encode the specified integer value into a span of bytes.

```csharp
    Span<byte> buffer = stackalloc byte[5];
    if( VariableIntegerEncoder.TryEncode( 42, buffer, out var bytesWritten ) )
    {
        Console.WriteLine( string.Join( ", ", buffer.Take( bytesWritten ) ) ); // 42
    }
```

- `Encode` Encodes the specified integer value into a byte array.

```csharp
    var encoded = VariableIntegerEncoder.Encode( int.MaxValue );
    Console.WriteLine( string.Join( ", ", encoded ) ); // 255, 255, 255, 255, 7
```

- `Decode` Decodes the specified byte span into an integer value.

```csharp
    var buffer = new byte[] { 255, 255, 255, 255, 7 };
    var decoded = VariableIntegerEncoder.Decode( buffer );
    Console.WriteLine( decoded ); // 2147483647
```

- `WriteEncoded` Writes the encoded integer value to the specified `Stream` object.

```csharp
    using( var stream = new MemoryStream() )
    {
        VariableIntegerEncoder.WriteEncoded( stream, int.MaxValue );
        Console.WriteLine( string.Join( ", ", stream.ToArray() ) ); // 255, 255, 255, 255, 7
    }
```

- `ReadEncoded` Reads an encoded integer value from the specified `Stream` object.

```csharp
    using( var stream = new MemoryStream( new byte[] { 255, 255, 255, 255, 7 } ) )
    {
        var decoded = VariableIntegerEncoder.ReadEncoded( stream );
        Console.WriteLine( decoded ); // 2147483647
    }
```

---

## License

This project is licensed under the [MIT License](LICENSE).
