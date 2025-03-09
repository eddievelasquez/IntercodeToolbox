﻿//HintName: GeneratorTest.Test.g.cs
// <auto-generated> This file has been auto generated by Intercode Toolbox Typed Primitives. </auto-generated>
#nullable enable

namespace GeneratorTest;

[global::System.Diagnostics.DebuggerDisplay( "Test = {_value}" )]
public readonly partial struct Test
  : global::Intercode.Toolbox.TypedPrimitives.IValueTypedPrimitive<global::System.TimeSpan, Test>,
    global::System.IComparable<Test>,
    global::System.IComparable<global::System.TimeSpan>,
    global::System.IComparable,
#if NET7_0_OR_GREATER
    global::System.ISpanFormattable,
    global::System.ISpanParsable<Test>
#else
    global::System.IFormattable,
    global::System.IParsable<Test>
#endif
{
  public static readonly Test Empty = new Test( null );

  private readonly global::System.TimeSpan? _value;

  private Test( global::System.TimeSpan? value )
  {
    _value = value;
    NormalizePartial( ref _value );
  }

  private global::System.TimeSpan GetWrappedValueUnsafe()
  {
    return _value.GetValueOrDefault();
  }

  public global::System.TimeSpan Value
  {
    get
    {
      if( !HasValue )
      {
        throw new global::System.InvalidOperationException( "Instance does not have a value" );
      }

      return GetWrappedValueUnsafe();
    }
  }

  public bool HasValue => _value.HasValue;

  public object? GetValueObject()
  {
    return GetValueOrDefault();
  }

  public global::System.TimeSpan? GetValueOrDefault()
  {
    return _value;
  }

  public global::System.TimeSpan GetValueOrDefault( global::System.TimeSpan defaultValue )
  {
    return HasValue ? GetWrappedValueUnsafe() : defaultValue;
  }

  public global::System.TimeSpan? ValueOrDefault => GetValueOrDefault();
  public bool IsDefault => !HasValue;

  public static global::System.Type GetUnderlyingType()
  {
    return typeof( global::System.TimeSpan );
  }

  public static global::FluentResults.Result<Test> Create( global::System.TimeSpan? value )
  {
    var result = Validate( value );
    if( result.IsFailed )
    {
      return global::FluentResults.Result.Fail<Test>( result.Errors );
    }

    return value.HasValue ? new Test( value.Value ) : Empty;
  }

  public static Test CreateOrThrow( global::System.TimeSpan? value )
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

  public static global::FluentResults.Result Validate( global::System.TimeSpan? value )
  {
    global::FluentResults.Result result = global::FluentResults.Result.Ok();
    ValidatePartial( value, ref result );
    return result;
  }

  public static void ValidateOrThrow( global::System.TimeSpan? value )
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

  public static bool IsValid( global::System.TimeSpan? value )
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
    return HasValue ? GetWrappedValueUnsafe().GetHashCode() : 0;
  }

  public override string ToString()
  {
    return HasValue ? GetWrappedValueUnsafe().ToString() : string.Empty;
  }

  public string ToString(
    string? format )
  {
    return HasValue ? GetWrappedValueUnsafe().ToString( format, null ) : string.Empty;
  }

  public string ToString(
    string? format,
    global::System.IFormatProvider? formatProvider )
  {
    return HasValue ? GetWrappedValueUnsafe().ToString( format, formatProvider ) : string.Empty;
  }

  public static Test Parse(
    string s, IFormatProvider? provider  )
  {
    var value = global::System.TimeSpan.Parse( s, provider );
    return CreateOrThrow( value );
  }

  public static bool TryParse(
    string? s,
    IFormatProvider? provider,
    out Test result )
  {
    if( !global::System.TimeSpan.TryParse( s, provider, out var value ) )
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
    if ( !HasValue )
    {
      charsWritten = 0;
      return true;
    }

    return GetWrappedValueUnsafe().TryFormat( destination, out charsWritten, format, provider );
  }

  public static Test Parse(
    global::System.ReadOnlySpan<char> text,
    global::System.IFormatProvider? formatProvider = null )
  {
    var value = global::System.TimeSpan.Parse( text, formatProvider );
    return CreateOrThrow( value );
  }

  public static bool TryParse(
    global::System.ReadOnlySpan<char> text,
    global::System.IFormatProvider? formatProvider,
    out Test result )
  {
    if ( !global::System.TimeSpan.TryParse( text, formatProvider, out var value ) )
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
    return obj switch
    {
      null => 1,
      Test primitive => CompareTo( primitive ),
      global::System.TimeSpan value => CompareTo( value ),
      _ => throw new global::System.ArgumentException( "Object is not a Test or global::System.TimeSpan" )
    };
  }

  public int CompareTo(
    Test other )
  {
    if ( !HasValue )
    {
      return !other.HasValue ? 0 : -1;
    }

    if ( !other.HasValue )
    {
      return 1;
    }

    return GetWrappedValueUnsafe().CompareTo( other.GetWrappedValueUnsafe() );
  }

  public int CompareTo(
    global::System.TimeSpan other )
  {
    if ( !HasValue )
    {
      return -1;
    }

    return GetWrappedValueUnsafe().CompareTo( other );
  }

  public static implicit operator global::System.TimeSpan(
    Test primitive )
  {
    return primitive.Value;
  }

  public static explicit operator Test( global::System.TimeSpan? value )
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
    ref global::System.TimeSpan? value );

  static partial void ValidatePartial(
    global::System.TimeSpan? value, ref global::FluentResults.Result result );
}
