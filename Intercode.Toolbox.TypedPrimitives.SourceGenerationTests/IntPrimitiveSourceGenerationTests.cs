namespace Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

using System.Threading.Tasks;
using Intercode.Toolbox.TypedPrimitives;
using Xunit;
using Xunit.Abstractions;

public class IntPrimitiveSourceGenerationTests( ITestOutputHelper output )
{
  #region Tests

  [Fact]
  public Task WithDefaultConvertersAndFlaglessValidator()
  {
    var source = """
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;

        using Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

        [TypedPrimitive(typeof( int ),
                        Converters = TypedPrimitiveConverter.Default,
                        ValidatorType = typeof( IntValidator ))]
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


        [TypedPrimitive(typeof( int ), Converters = TypedPrimitiveConverter.Default)]
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

        [TypedPrimitive(typeof( int ),
                        Converters = TypedPrimitiveConverter.Default,
                        ValidatorType = typeof( IntValidator ),
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

        [TypedPrimitive(typeof( int ),
                        Converters = TypedPrimitiveConverter.Default,
                        ValidatorType = typeof( IntValidator ),
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

        [TypedPrimitive(typeof( int ),
                        Converters = TypedPrimitiveConverter.None,
                        ValidatorType = typeof( IntValidator ))]
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


        [TypedPrimitive(typeof( int ), Converters = TypedPrimitiveConverter.None)]
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

        [TypedPrimitive(typeof( int ),
                        Converters = TypedPrimitiveConverter.None,
                        ValidatorType = typeof( IntValidator ),
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

        [TypedPrimitive(typeof( int ),
                        Converters = TypedPrimitiveConverter.None,
                        ValidatorType = typeof( IntValidator ),
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

        [TypedPrimitive(typeof( int ),
                        ValidatorType = typeof( IntValidator ))]
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


        [TypedPrimitive(typeof( int ))]
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

        [TypedPrimitive(typeof( int ),
                        ValidatorType = typeof( IntValidator ),
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

        [TypedPrimitive(typeof( int ),
                        ValidatorType = typeof( IntValidator ),
                        ValidatorFlagsType = typeof( ValidatorFlags ))]
        public readonly partial record struct Test;
      """;

    return SourceGeneratorTestHelper.Verify<TypedPrimitiveSourceGenerator>( source, output );
  }

  #endregion
}
