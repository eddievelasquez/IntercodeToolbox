﻿//HintName: GeneratorTest.Test.g.cs
// <auto-generated> This file has been auto generated by Intercode Toolbox Typed Primitives. </auto-generated>
#nullable enable

namespace GeneratorTest;

[global::System.ComponentModel.TypeConverter( typeof( GeneratorTest.TestTypeConverter ) )]
[global::System.Text.Json.Serialization.JsonConverter( typeof( GeneratorTest.TestSystemTextJsonConverter ) )]
[global::Newtonsoft.Json.JsonConverter( typeof( GeneratorTest.TestNewtonsoftJsonConverter ) )]
[global::System.Diagnostics.DebuggerDisplay( "Test = {_value}" )]
public readonly partial struct Test
  : global::Intercode.Toolbox.TypedPrimitives.IValueTypePrimitive<Test, long>,
    global::System.IComparable<Test>,
    global::System.IComparable,
#if NET7_0_OR_GREATER
    global::System.ISpanFormattable,
    global::System.ISpanParsable<Test>
#else
    global::System.IFormattable,
    global::System.IParsable<Test>
#endif
{
  private readonly long? _value;

  private Test( long? value )
  {
    _value = value;
    NormalizePartial( ref _value );
  }

  public long Value
  {
    get
    {
      if( _value is null )
      {
        throw new global::System.InvalidOperationException( "Value is null" );
      }

      return _value.Value;
    }
  }

  public long? ValueOrDefault => _value;
  public bool IsDefault => _value is null;

  public static global::System.Type GetPrimitiveType()
  {
    return typeof( long );
  }

  public static global::FluentResults.Result<Test> Create( long? value )
  {
    var result = Validate( value );
    if( result.IsFailed )
    {
      return global::FluentResults.Result.Fail<Test>( result.Errors );
    }

    return new Test( value );
  }

  public static Test CreateOrThrow( long? value )
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

  public static global::FluentResults.Result Validate( long? value )
  {
    global::FluentResults.Result result = global::FluentResults.Result.Ok();
    ValidatePartial( value, ref result );
    return result;
  }

  public static void ValidateOrThrow( long? value )
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

  public static bool IsValid( long? value )
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
    return _value is null ? string.Empty : _value.Value.ToString( format, null );
  }

  public string ToString(
    string? format,
    global::System.IFormatProvider? formatProvider )
  {
    return _value is null ? string.Empty : _value.Value.ToString( format, formatProvider );
  }

  public static Test Parse(
    string s, IFormatProvider? provider  )
  {
    var value = long.Parse( s, provider );
    return CreateOrThrow( value );
  }

  public static bool TryParse(
    string? s,
    IFormatProvider? provider,
    out Test result )
  {
    if( !long.TryParse( s, provider, out var value ) )
    {
      result = default;
      return false;
    }

    var createResult = Create( value );
    if ( createResult.IsFailed )
    {
      result = default;
      return false;
    }

    result = createResult.Value;
    return true;
  }

#if NET7_0_OR_GREATER
  public bool TryFormat(
    global::System.Span<char> destination,
    out int charsWritten,
    global::System.ReadOnlySpan<char> format,
    global::System.IFormatProvider? provider )
  {
    if ( _value is null )
    {
      charsWritten = 0;
      return true;
    }

    return _value.Value.TryFormat( destination, out charsWritten, format, provider );
  }

  public static Test Parse(
    global::System.ReadOnlySpan<char> text,
    global::System.IFormatProvider? formatProvider = null )
  {
    var value = long.Parse( text, formatProvider );
    return CreateOrThrow( value );
  }

  public static bool TryParse(
    global::System.ReadOnlySpan<char> text,
    global::System.IFormatProvider? formatProvider,
    out Test result )
  {
    if ( !long.TryParse( text, formatProvider, out var value ) )
    {
      result = default;
      return false;
    }

    var createResult = Create( value );
    if ( createResult.IsFailed )
    {
      result = default;
      return false;
    }

    result = createResult.Value;
    return true;
  }

#endif

  public int CompareTo(
    object? obj )
  {
    if( obj is Test primitive )
    {
      return CompareTo( primitive );
    }

    return 1;
  }

  public int CompareTo(
    Test other )
  {
    if( _value is null )
    {
      return other._value is null ? 0 : -1;
    }

    if( other._value is null )
    {
      return 1;
    }

    return _value.Value.CompareTo( other.Value );
  }

  public static implicit operator long(
    Test primitive )
  {
    return primitive.Value;
  }

  public static explicit operator Test( long? value )
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
    ref long? value );

  static partial void ValidatePartial(
    long? value, ref global::FluentResults.Result result );
}
