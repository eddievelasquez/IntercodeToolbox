﻿//HintName: GeneratorTest.Test.g.cs
// <auto-generated> This file has been auto generated by Intercode Toolbox Typed Primitives. </auto-generated>
#nullable enable

namespace GeneratorTest;

[global::System.Diagnostics.DebuggerDisplay( "Test = {_value}" )]
public readonly partial struct Test
  : global::Intercode.Toolbox.TypedPrimitives.IValueTypedPrimitive<ushort, Test>,
    global::System.IComparable<Test>,
    global::System.IComparable<ushort>,
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

  private readonly ushort? _value;

  private Test( ushort? value )
  {
    _value = value;
    NormalizePartial( ref _value );
  }

  private ushort GetWrappedValueUnsafe()
  {
    return _value.GetValueOrDefault();
  }

  public ushort Value
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

  public ushort? GetValueOrDefault()
  {
    return _value;
  }

  public ushort GetValueOrDefault( ushort defaultValue )
  {
    return HasValue ? GetWrappedValueUnsafe() : defaultValue;
  }

  public ushort? ValueOrDefault => GetValueOrDefault();
  public bool IsDefault => !HasValue;

  public static global::System.Type GetUnderlyingType()
  {
    return typeof( ushort );
  }

  public static global::FluentResults.Result<Test> Create( ushort? value )
  {
    var result = Validate( value );
    if( result.IsFailed )
    {
      return global::FluentResults.Result.Fail<Test>( result.Errors );
    }

    return value.HasValue ? new Test( value.Value ) : Empty;
  }

  public static Test CreateOrThrow( ushort? value )
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

  public static global::FluentResults.Result Validate( ushort? value )
  {
    global::FluentResults.Result result = global::FluentResults.Result.Ok();
    ValidatePartial( value, ref result );
    return result;
  }

  public static void ValidateOrThrow( ushort? value )
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

  public static bool IsValid( ushort? value )
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
    var value = ushort.Parse( s, provider );
    return CreateOrThrow( value );
  }

  public static bool TryParse(
    string? s,
    IFormatProvider? provider,
    out Test result )
  {
    if( !ushort.TryParse( s, provider, out var value ) )
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
    var value = ushort.Parse( text, formatProvider );
    return CreateOrThrow( value );
  }

  public static bool TryParse(
    global::System.ReadOnlySpan<char> text,
    global::System.IFormatProvider? formatProvider,
    out Test result )
  {
    if ( !ushort.TryParse( text, formatProvider, out var value ) )
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
      ushort value => CompareTo( value ),
      _ => throw new global::System.ArgumentException( "Object is not a Test or ushort" )
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
    ushort other )
  {
    if ( !HasValue )
    {
      return -1;
    }

    return GetWrappedValueUnsafe().CompareTo( other );
  }

  public static implicit operator ushort(
    Test primitive )
  {
    return primitive.Value;
  }

  public static explicit operator Test( ushort? value )
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
    ref ushort? value );

  static partial void ValidatePartial(
    ushort? value, ref global::FluentResults.Result result );
}
