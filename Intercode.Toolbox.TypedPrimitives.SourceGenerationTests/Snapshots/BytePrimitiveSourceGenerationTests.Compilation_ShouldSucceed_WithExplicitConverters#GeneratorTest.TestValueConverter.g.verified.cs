﻿//HintName: GeneratorTest.TestValueConverter.g.cs
// <auto-generated> This file has been auto generated by Intercode Toolbox Typed Primitives. </auto-generated>
#nullable enable

namespace GeneratorTest;

public class TestValueConverter: global::Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<GeneratorTest.Test, byte>
{
  public TestValueConverter()
    : this( null )
  {
  }

  public TestValueConverter(
    global::Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints? mappingHints = null )
    : base(
      primitive => primitive.Value,
      value => ( GeneratorTest.Test ) value,
      mappingHints
    )
  {
  }
}
