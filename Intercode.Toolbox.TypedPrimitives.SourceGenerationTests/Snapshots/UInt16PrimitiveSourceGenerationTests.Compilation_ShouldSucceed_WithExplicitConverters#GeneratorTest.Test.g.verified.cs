﻿//HintName: GeneratorTest.Test.g.cs
// <auto-generated> This file has been auto generated by Intercode Toolbox Typed Primitives. </auto-generated>
#nullable enable

namespace GeneratorTest;

[global::System.ComponentModel.TypeConverter( typeof( GeneratorTest.TestTypeConverter ) )]
[global::System.Text.Json.Serialization.JsonConverter( typeof( GeneratorTest.TestSystemTextJsonConverter ) )]
[global::Newtonsoft.Json.JsonConverter( typeof( GeneratorTest.TestNewtonsoftJsonConverter ) )]
[global::System.Diagnostics.DebuggerDisplay( "Test = {_value}" )]
public readonly partial struct Test
  : global::System.IComparable<Test>,
    global::System.IComparable,
    global::System.IFormattable
{
  private readonly ushort? _value;

  private Test( ushort? value )
  {
    _value = value;
    NormalizePartial( ref _value );
  }

  public ushort Value
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

  public ushort? ValueOrDefault => _value;
  public bool IsDefault => _value is null;

  public static global::FluentResults.Result<Test> Create( ushort? value )
  {
    var result = Validate( value );
    if( result.IsFailed )
    {
      return global::FluentResults.Result.Fail<Test>( result.Errors );
    }

    return new Test( value );
  }

  public static global::FluentResults.Result Validate( ushort? value )
  {
    global::FluentResults.Result result = global::FluentResults.Result.Ok();
    ValidatePartial( value, ref result );
    return result;
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

    return ((ushort) _value).CompareTo( ((ushort) other) );
  }

  public static explicit operator ushort(
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
