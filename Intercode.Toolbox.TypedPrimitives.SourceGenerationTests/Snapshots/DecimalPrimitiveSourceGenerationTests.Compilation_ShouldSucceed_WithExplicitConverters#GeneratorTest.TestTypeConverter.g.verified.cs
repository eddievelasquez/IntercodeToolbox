﻿//HintName: GeneratorTest.TestTypeConverter.g.cs
// <auto-generated> This file has been auto generated by Intercode Toolbox Typed Primitives. </auto-generated>
#nullable enable

namespace GeneratorTest;

public partial class TestTypeConverter: global::System.ComponentModel.TypeConverter
{
  public override bool CanConvertFrom(
    global::System.ComponentModel.ITypeDescriptorContext? context,
    global::System.Type sourceType )
  {
    if ( sourceType == typeof( double ) )
    {
      return true;
    }

    bool canConvert = false;
    CanConvertFromPartial( context, sourceType, ref canConvert );
    return canConvert;
  }

  public override bool CanConvertTo(
    global::System.ComponentModel.ITypeDescriptorContext? context,
    global::System.Type? destinationType )
  {
    if ( destinationType == typeof( double ) )
    {
      return true;
    }

    bool canConvert = false;
    CanConvertToPartial( context, destinationType, ref canConvert );
    return canConvert;
  }

  public override object? ConvertFrom(
    global::System.ComponentModel.ITypeDescriptorContext? context,
    global::System.Globalization.CultureInfo? culture,
    object value )
  {
    if ( value is null )
    {
      return ( GeneratorTest.Test ) default;
    }

    if ( value is double rawValue )
    {
      return ( GeneratorTest.Test ) rawValue;
    }

    bool converted = false;
    double? convertedValue = null;
    ConvertFromPartial( context, culture, value, ref convertedValue, ref converted );

    if ( !converted )
    {
      throw new NotSupportedException( string.Format( "Converting from {0} is not supported", value.GetType() ) );
    }

    return ( GeneratorTest.Test ) convertedValue;
  }

  public override object? ConvertTo(
    global::System.ComponentModel.ITypeDescriptorContext? context,
    global::System.Globalization.CultureInfo? culture,
    object? value,
    global::System.Type destinationType )
  {
    if( value is GeneratorTest.Test typedPrimitive )
    {
      if( destinationType == typeof( double ) )
      {
        return typedPrimitive.GetValueOrDefault();
      }

      bool converted = false;
      object? convertedValue = null;
      ConvertToPartial( context, culture, typedPrimitive.GetValueOrDefault(), destinationType, ref convertedValue, ref converted );

      if ( !converted )
      {
        throw new NotSupportedException( string.Format( "Converting to {0} is not supported", destinationType ) );
      }

      return convertedValue;
    }

    return base.ConvertTo( context, culture, value, destinationType );
  }

  partial void CanConvertFromPartial(
    global::System.ComponentModel.ITypeDescriptorContext? context,
    global::System.Type sourceType,
    ref bool canConvert );

  partial void CanConvertToPartial(
    global::System.ComponentModel.ITypeDescriptorContext? context,
    global::System.Type? destinationType,
    ref bool canConvert );

  partial void ConvertFromPartial(
    global::System.ComponentModel.ITypeDescriptorContext? context,
    global::System.Globalization.CultureInfo? culture,
    object value,
    ref double? convertedValue,
    ref bool converted );

  partial void ConvertToPartial(
    global::System.ComponentModel.ITypeDescriptorContext? context,
    global::System.Globalization.CultureInfo? culture,
    double? value,
    global::System.Type destinationType,
    ref object? convertedValue,
    ref bool converted );
}
