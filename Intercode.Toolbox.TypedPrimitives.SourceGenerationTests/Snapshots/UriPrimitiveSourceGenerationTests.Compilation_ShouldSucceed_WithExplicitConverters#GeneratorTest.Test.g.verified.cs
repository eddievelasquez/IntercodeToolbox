﻿//HintName: GeneratorTest.Test.g.cs
// <auto-generated> This file has been auto generated by Intercode Toolbox Typed Primitives. </auto-generated>
#nullable enable

namespace GeneratorTest;

[global::System.ComponentModel.TypeConverter( typeof( GeneratorTest.TestTypeConverter ) )]
[global::System.Text.Json.Serialization.JsonConverter( typeof( GeneratorTest.TestSystemTextJsonConverter ) )]
[global::Newtonsoft.Json.JsonConverter( typeof( GeneratorTest.TestNewtonsoftJsonConverter ) )]
[global::System.Diagnostics.DebuggerDisplay( "Test = {_value}" )]
public readonly partial struct Test
  : global::Intercode.Toolbox.TypedPrimitives.IReferenceTypePrimitive<Test, global::System.Uri>,
    global::System.IComparable<Test>,
    global::System.IComparable<global::System.Uri>,
    global::System.IComparable,
#if NET7_0_OR_GREATER
    global::System.ISpanFormattable
#else
    global::System.IFormattable
#endif
{
  public static readonly Test Empty = new Test( null );

  private readonly global::System.Uri? _value;

  private Test( global::System.Uri? value )
  {
    _value = value;
    NormalizePartial( ref _value );
  }

  public global::System.Uri Value
  {
    get
    {
      if( _value is null )
      {
        throw new global::System.InvalidOperationException( "Value is null" );
      }

      return _value;
    }
  }

  public bool HasValue => _value is not null;

  public global::System.Uri? GetValueOrDefault()
  {
    return _value;
  }

  public global::System.Uri? GetValueOrDefault( global::System.Uri defaultValue )
  {
    return HasValue ? _value : defaultValue;
  }

  public global::System.Uri? ValueOrDefault => _value;
  public bool IsDefault => _value is null;

  public static global::System.Type GetPrimitiveType()
  {
    return typeof( global::System.Uri );
  }

  public static global::FluentResults.Result<Test> Create( global::System.Uri? value )
  {
    var result = Validate( value );
    if( result.IsFailed )
    {
      return global::FluentResults.Result.Fail<Test>( result.Errors );
    }

    return new Test( value );
  }

  public static Test CreateOrThrow( global::System.Uri? value )
  {
    var result = Create( value );
    if( result.IsSuccess )
    {
      return result.Value;
    }

    throw new global::System.ArgumentException(
      global::System.Linq.Enumerable.First( result.Errors )
            .Message
    );
  }

  public static global::FluentResults.Result Validate( global::System.Uri? value )
  {
    global::FluentResults.Result result = global::FluentResults.Result.Ok();
    ValidatePartial( value, ref result );
    return result;
  }

  public static void ValidateOrThrow( global::System.Uri? value )
  {
    var result = Validate( value );
    if( result.IsSuccess )
    {
      return;
    }

    throw new global::System.ArgumentException(
      global::System.Linq.Enumerable.First( result.Errors )
            .Message
    );
  }

  public static bool IsValid( global::System.Uri? value )
  {
    return Validate( value ).IsSuccess;
  }

  public bool Equals(
    Test other )
  {
    return CompareTo( other ) == 0;
  }

  public override int GetHashCode()
  {
    return _value is null ? 0 : _value.GetHashCode();
  }

  public override string ToString()
  {
    return _value is null ? string.Empty : _value.ToString()!;
  }

  public string ToString(
    string? format )
  {
    return _value is null ? string.Empty : ((IFormattable) _value).ToString( format, null );
  }

  public string ToString(
    string? format,
    global::System.IFormatProvider? formatProvider )
  {
    return _value is null ? string.Empty : ((IFormattable) _value).ToString( format, formatProvider );
  }

#if NET7_0_OR_GREATER
  bool ISpanFormattable.TryFormat(
    global::System.Span<char> destination,
    out int charsWritten,
    global::System.ReadOnlySpan<char> format,
    global::System.IFormatProvider? provider )
  {
    return TryFormat( destination, out charsWritten );
  }

  public bool TryFormat(
    global::System.Span<char> destination,
    out int charsWritten )
  {
    if ( _value is null )
    {
      charsWritten = 0;
      return true;
    }

    return _value.TryFormat( destination, out charsWritten );
  }
#endif

  public int CompareTo(
    object? obj )
  {
    return obj switch
    {
      null => 1,
      Test primitive => CompareTo( primitive ),
      global::System.Uri value => CompareTo( _value ),
      _ => throw new global::System.ArgumentException( "Object is not a Test or global::System.Uri" )
    };
  }

  public int CompareTo(
    Test other )
  {
    return CompareTo( other.GetValueOrDefault() );
  }

  public int CompareTo(
    global::System.Uri? other )
  {
    if( _value is null )
    {
      return other is null ? 0 : -1;
    }

    if( other is null )
    {
      return 1;
    }

    return Uri.Compare( _value, other, UriComponents.AbsoluteUri, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase );
  }

  public static implicit operator global::System.Uri(
    Test primitive )
  {
    return primitive.Value;
  }

  public static explicit operator Test( global::System.Uri? value )
  {
    var result = Test.Create( value );
    if( result.IsFailed )
    {
      throw new global::System.InvalidOperationException(
        global::System.Linq.Enumerable.First( result.Errors )
              .Message
      );
    }

    return result.Value;
  }

  static partial void NormalizePartial(
    ref global::System.Uri? value );

  static partial void ValidatePartial(
    global::System.Uri? value, ref global::FluentResults.Result result );
}
