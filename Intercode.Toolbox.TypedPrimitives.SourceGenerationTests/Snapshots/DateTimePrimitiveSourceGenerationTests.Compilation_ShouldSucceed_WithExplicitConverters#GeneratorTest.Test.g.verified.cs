﻿//HintName: GeneratorTest.Test.g.cs
// <auto-generated> This file has been auto generated by Intercode Toolbox Typed Primitives. </auto-generated>
#nullable enable

namespace GeneratorTest;

[global::System.ComponentModel.TypeConverter( typeof( GeneratorTest.TestTypeConverter ) )]
[global::System.Text.Json.Serialization.JsonConverter( typeof( GeneratorTest.TestSystemTextJsonConverter ) )]
[global::System.Diagnostics.DebuggerDisplay( "Test = {_value}" )]
public readonly partial struct Test
  : global::System.IComparable<Test>,
    global::System.IComparable,
    global::System.IFormattable
{
  private readonly global::System.DateTime? _value;

  private Test( global::System.DateTime? value )
  {
    _value = value;
    NormalizePartial( ref _value );
  }

  public global::System.DateTime Value
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

  public global::System.DateTime? ValueOrDefault => _value;
  public bool IsDefault => _value is null;

  public static global::FluentResults.Result<Test> Create( global::System.DateTime? value )
  {
    var result = Validate( value );
    if( result.IsFailed )
    {
      return global::FluentResults.Result.Fail<Test>( result.Errors );
    }

    return new Test( value );
  }

  public static global::FluentResults.Result Validate( global::System.DateTime? value )
  {
    global::FluentResults.Result result = global::FluentResults.Result.Ok();
    ValidatePartial( value, ref result );
    return result;
  }

  public static bool IsValid( global::System.DateTime? value )
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

    return ((global::System.DateTime) _value).CompareTo( ((global::System.DateTime) other) );
  }

  public static explicit operator global::System.DateTime(
    Test primitive )
  {
    return primitive.Value;
  }

  public static explicit operator Test( global::System.DateTime? value )
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
    ref global::System.DateTime? value );

  static partial void ValidatePartial(
    global::System.DateTime? value, ref global::FluentResults.Result result );
}
