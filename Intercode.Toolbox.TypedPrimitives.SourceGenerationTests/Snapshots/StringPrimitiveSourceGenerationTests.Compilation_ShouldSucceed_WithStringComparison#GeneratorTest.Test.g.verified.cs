﻿//HintName: GeneratorTest.Test.g.cs
// <auto-generated> This file has been auto generated by Intercode Toolbox Typed Primitives. </auto-generated>
#nullable enable

namespace GeneratorTest;

[global::System.Diagnostics.DebuggerDisplay( "Test = {_value}" )]
public readonly partial struct Test
  : global::Intercode.Toolbox.TypedPrimitives.IReferenceTypedPrimitive<string, Test>,
    global::System.IComparable<Test>,
    global::System.IComparable<string>,
    global::System.IComparable
{
  public static readonly Test Empty = new Test( null );

  private readonly string? _value;

  private Test( string? value )
  {
    _value = value;
    NormalizePartial( ref _value );
  }

  public string Value
  {
    get
    {
      if( !HasValue )
      {
        throw new global::System.InvalidOperationException( "Instance does not have a value" );
      }

      return _value!;
    }
  }

  public bool HasValue => _value is not null;

  public object? GetValueObject()
  {
    return GetValueOrDefault();
  }

  public string? GetValueOrDefault()
  {
    return _value;
  }

  public string GetValueOrDefault( string defaultValue )
  {
    return HasValue ? _value! : defaultValue!;
  }

  public string? ValueOrDefault => GetValueOrDefault();
  public bool IsDefault => !HasValue;

  public static global::System.Type GetUnderlyingType()
  {
    return typeof( string );
  }

  public static global::FluentResults.Result<Test> Create( string? value )
  {
    var result = Validate( value );
    if( result.IsFailed )
    {
      return global::FluentResults.Result.Fail<Test>( result.Errors );
    }

    return new Test( value );
  }

  public static Test CreateOrThrow( string? value )
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

  public static global::FluentResults.Result Validate( string? value )
  {
    global::FluentResults.Result result = global::FluentResults.Result.Ok();
    ValidatePartial( value, ref result );
    return result;
  }

  public static void ValidateOrThrow( string? value )
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

  public static bool IsValid( string? value )
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
    return _value is null ? string.Empty : _value;
  }

  public string ToString(
    global::System.IFormatProvider? formatProvider )
  {
    return _value is null ? string.Empty : _value.ToString( formatProvider );
  }

  public int CompareTo(
    object? obj )
  {
    return obj switch
    {
      null => 1,
      Test primitive => CompareTo( primitive ),
      string value => CompareTo( _value ),
      _ => throw new global::System.ArgumentException( "Object is not a Test or string" )
    };
  }

  public int CompareTo(
    Test other )
  {
    return CompareTo( other.GetValueOrDefault() );
  }

  public int CompareTo(
    string? other )
  {
    if( _value is null )
    {
      return other is null ? 0 : -1;
    }

    if( other is null )
    {
      return 1;
    }

    return string.Compare( _value, other, System.StringComparison.Ordinal );
  }

  public static implicit operator string(
    Test primitive )
  {
    return primitive.Value;
  }

  public static explicit operator Test( string? value )
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
    ref string? value );

  static partial void ValidatePartial(
    string? value, ref global::FluentResults.Result result );
}
