namespace Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

using System.Threading.Tasks;
using Intercode.Toolbox.TypedPrimitives;
using Xunit;
using Xunit.Abstractions;

public class StringPrimitiveSourceGenerationTests( ITestOutputHelper output )
{
  #region Tests

  [Fact]
  public Task OrdinalCaseSensitiveComparisons()
  {
    var source = """
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;

        using Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

        [TypedPrimitive(typeof( string ),
                        ValidatorType = typeof( StringValidator ),
                        ValidatorFlagsType = typeof( ValidatorFlags ),
                        ValidatorFlagsDefaultValue = ValidatorFlags.Full,
                        StringComparison = System.StringComparison.Ordinal)]
        public readonly partial record struct Test;
      """;

    return SourceGeneratorTestHelper.Verify<TypedPrimitiveSourceGenerator>( source, output );
  }

  [Fact]
  public Task WithDefaultConvertersAndFlaglessValidator()
  {
    var source = """
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;

        using Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

        [TypedPrimitive(typeof( string ),
                        Converters = TypedPrimitiveConverter.Default,
                        ValidatorType = typeof( StringValidator ))]
        public readonly partial record struct Test;
      """;

    return SourceGeneratorTestHelper.Verify<TypedPrimitiveSourceGenerator>( source, output );
  }

  [Fact]
  public Task WithDefaultConvertersAndNoValidation()
  {
    var source = """
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;


        [TypedPrimitive(typeof( string ), Converters = TypedPrimitiveConverter.Default)]
        public readonly partial record struct Test;
      """;

    return SourceGeneratorTestHelper.Verify<TypedPrimitiveSourceGenerator>( source, output );
  }

  [Fact]
  public Task WithDefaultConvertersAndValidatorWithDefaultFlagValue()
  {
    var source = """
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;

        using Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

        [TypedPrimitive(typeof( string ),
                        Converters = TypedPrimitiveConverter.Default,
                        ValidatorType = typeof( StringValidator ),
                        ValidatorFlagsType = typeof( ValidatorFlags ),
                        ValidatorFlagsDefaultValue = ValidatorFlags.Full )]
        public readonly partial record struct Test;
      """;

    return SourceGeneratorTestHelper.Verify<TypedPrimitiveSourceGenerator>( source, output );
  }

  [Fact]
  public Task WithDefaultConvertersAndValidatorWithoutDefaultFlagValue()
  {
    var source = """
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;

        using Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

        [TypedPrimitive(typeof( string ),
                        Converters = TypedPrimitiveConverter.Default,
                        ValidatorType = typeof( StringValidator ),
                        ValidatorFlagsType = typeof( ValidatorFlags ))]
        public readonly partial record struct Test;
      """;

    return SourceGeneratorTestHelper.Verify<TypedPrimitiveSourceGenerator>( source, output );
  }

  [Fact]
  public Task WithNoConvertersAndFlaglessValidator()
  {
    var source = """
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;

        using Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

        [TypedPrimitive(typeof( string ),
                        Converters = TypedPrimitiveConverter.None,
                        ValidatorType = typeof( StringValidator ))]
        public readonly partial record struct Test;
      """;

    return SourceGeneratorTestHelper.Verify<TypedPrimitiveSourceGenerator>( source, output );
  }

  [Fact]
  public Task WithNoConvertersAndNoValidation()
  {
    var source = """
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;


        [TypedPrimitive(typeof( string ), Converters = TypedPrimitiveConverter.None)]
        public readonly partial record struct Test;
      """;

    return SourceGeneratorTestHelper.Verify<TypedPrimitiveSourceGenerator>( source, output );
  }

  [Fact]
  public Task WithNoConvertersAndValidatorWithDefaultFlagValue()
  {
    var source = """
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;

        using Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

        [TypedPrimitive(typeof( string ),
                        Converters = TypedPrimitiveConverter.None,
                        ValidatorType = typeof( StringValidator ),
                        ValidatorFlagsType = typeof( ValidatorFlags ),
                        ValidatorFlagsDefaultValue = ValidatorFlags.Full )]
        public readonly partial record struct Test;
      """;

    return SourceGeneratorTestHelper.Verify<TypedPrimitiveSourceGenerator>( source, output );
  }

  [Fact]
  public Task WithNoConvertersAndValidatorWithoutDefaultFlagValue()
  {
    var source = """
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;

        using Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

        [TypedPrimitive(typeof( string ),
                        Converters = TypedPrimitiveConverter.None,
                        ValidatorType = typeof( StringValidator ),
                        ValidatorFlagsType = typeof( ValidatorFlags ))]
        public readonly partial record struct Test;
      """;

    return SourceGeneratorTestHelper.Verify<TypedPrimitiveSourceGenerator>( source, output );
  }

  [Fact]
  public Task WithoutExplicitConvertersAndFlaglessValidator()
  {
    var source = """
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;

        using Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

        [TypedPrimitive(typeof( string ),
                        ValidatorType = typeof( StringValidator ))]
        public readonly partial record struct Test;
      """;

    return SourceGeneratorTestHelper.Verify<TypedPrimitiveSourceGenerator>( source, output );
  }

  [Fact]
  public Task WithoutExplicitConvertersAndNoValidation()
  {
    var source = """
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;


        [TypedPrimitive(typeof( string ))]
        public readonly partial record struct Test;
      """;

    return SourceGeneratorTestHelper.Verify<TypedPrimitiveSourceGenerator>( source, output );
  }

  [Fact]
  public Task WithoutExplicitConvertersAndValidatorWithDefaultFlagValue()
  {
    var source = """
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;

        using Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

        [TypedPrimitive(typeof( string ),
                        ValidatorType = typeof( StringValidator ),
                        ValidatorFlagsType = typeof( ValidatorFlags ),
                        ValidatorFlagsDefaultValue = ValidatorFlags.Full )]
        public readonly partial record struct Test;
      """;

    return SourceGeneratorTestHelper.Verify<TypedPrimitiveSourceGenerator>( source, output );
  }

  [Fact]
  public Task WithoutExplicitConvertersAndValidatorWithoutDefaultFlagValue()
  {
    var source = """
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;

        using Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

        [TypedPrimitive(typeof( string ),
                        ValidatorType = typeof( StringValidator ),
                        ValidatorFlagsType = typeof( ValidatorFlags ))]
        public readonly partial record struct Test;
      """;

    return SourceGeneratorTestHelper.Verify<TypedPrimitiveSourceGenerator>( source, output );
  }

  #endregion
}
