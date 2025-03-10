﻿//HintName: GeneratorTest.TestNewtonsoftJsonConverter.g.cs
// <auto-generated> This file has been auto generated by Intercode Toolbox Typed Primitives. </auto-generated>
#nullable enable

namespace GeneratorTest;

public partial class TestNewtonsoftJsonConverter: global::Newtonsoft.Json.JsonConverter
{
  public override bool CanConvert(
    global::System.Type objectType )
  {
    return objectType == typeof( GeneratorTest.Test );
  }

  public override object? ReadJson(
    global::Newtonsoft.Json.JsonReader reader,
    global::System.Type objectType,
    object? existingValue,
    global::Newtonsoft.Json.JsonSerializer serializer )
  {
    float? value = null;
    if( reader.TokenType != global::Newtonsoft.Json.JsonToken.Null )
    {
      if( reader.TokenType == global::Newtonsoft.Json.JsonToken.Float )
      {
        value = global::System.Convert.ToSingle( reader.Value! );
      }
      else
      {
        var converted = false;
        ConvertToPartial( ref reader, objectType, ref value, ref converted );

        if( !converted )
        {
          throw new global::Newtonsoft.Json.JsonSerializationException( "Value must be a Float" );
        }
      }
    }

    var result = GeneratorTest.Test.Create( value );
    if( result.IsFailed )
    {
      throw new global::Newtonsoft.Json.JsonSerializationException( global::System.Linq.Enumerable.First( result.Errors ).Message );
    }

    return result.Value;
  }

  public override void WriteJson(
    global::Newtonsoft.Json.JsonWriter writer,
    object? value,
    global::Newtonsoft.Json.JsonSerializer serializer )
  {
    if( value is null )
    {
      writer.WriteNull();
      return;
    }

    if( value is not GeneratorTest.Test s )
    {
      throw new global::Newtonsoft.Json.JsonSerializationException( $"Unexpected object type: {value.GetType().Name}" );
    }

    if( !s.HasValue )
    {
      writer.WriteNull();
      return;
    }

    writer.WriteValue( s.Value );
  }

  partial void ConvertToPartial(
    ref global::Newtonsoft.Json.JsonReader reader,
    global::System.Type typeToConvert,
    ref float? convertedValue,
    ref bool converted );
}
