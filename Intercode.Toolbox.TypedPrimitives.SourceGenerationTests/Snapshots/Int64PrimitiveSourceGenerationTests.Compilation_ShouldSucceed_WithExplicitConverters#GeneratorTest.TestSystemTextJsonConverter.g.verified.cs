﻿//HintName: GeneratorTest.TestSystemTextJsonConverter.g.cs
// <auto-generated> This file has been auto generated by Intercode Toolbox Typed Primitives. </auto-generated>
#nullable enable

namespace GeneratorTest;

public partial class TestSystemTextJsonConverter: global::System.Text.Json.Serialization.JsonConverter<GeneratorTest.Test>
{
  public override bool CanConvert(
    global::System.Type typeToConvert )
  {
    return typeToConvert == typeof( GeneratorTest.Test );
  }

  public override GeneratorTest.Test Read(
    ref global::System.Text.Json.Utf8JsonReader reader,
    global::System.Type typeToConvert,
    global::System.Text.Json.JsonSerializerOptions options )
  {
    long? value = null;
    if( reader.TokenType != global::System.Text.Json.JsonTokenType.Null )
    {
      if( reader.TokenType == global::System.Text.Json.JsonTokenType.Number )
      {
        value = reader.GetInt64();
      }
      else
      {
        bool converted = false;
        ConvertToPartial( ref reader, typeToConvert, options, ref value, ref converted );

        if ( !converted )
        {
          throw new global::System.Text.Json.JsonException( "Value must be a Number" );
        }
      }
    }

    var result = GeneratorTest.Test.Create( value );
    if( result.IsFailed )
    {
      throw new global::System.Text.Json.JsonException(
        global::System.Linq.Enumerable.First( result.Errors )
              .Message
      );
    }

    return result.Value;
  }

  public override void Write(
    global::System.Text.Json.Utf8JsonWriter writer,
    GeneratorTest.Test value,
    global::System.Text.Json.JsonSerializerOptions options )
  {
    if ( !value.HasValue )
    {
      writer.WriteNullValue();
      return;
    }

    writer.WriteNumberValue( value.Value );
  }

  partial void ConvertToPartial(
    ref global::System.Text.Json.Utf8JsonReader reader,
    global::System.Type typeToConvert,
    global::System.Text.Json.JsonSerializerOptions options,
    ref long? value,
    ref bool converted );
}
