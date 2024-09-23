namespace Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

using System.Threading.Tasks;
using Intercode.Toolbox.TypedPrimitives;
using Xunit;
using Xunit.Abstractions;

public class DateTimePrimitiveSourceGenerationTests( ITestOutputHelper output )
{
  #region Tests

  [Fact]
  public Task WithDefaultConvertersAndFlaglessValidator()
  {
    var source = """
        namespace GeneratorTest;

        using Intercode.Toolbox.TypedPrimitives;

        using Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

        [TypedPrimitive(typeof( System.DateTime ),
                        Converters = TypedPrimitiveConverter.Default,
                        ValidatorType = typeof( DateTimeValidator ))]
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


        [TypedPrimitive(typeof( System.DateTime ), Converters = TypedPrimitiveConverter.Default)]
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

        [TypedPrimitive(typeof( System.DateTime ),
                        Converters = TypedPrimitiveConverter.Default,
                        ValidatorType = typeof( DateTimeValidator ),
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

        [TypedPrimitive(typeof( System.DateTime ),
                        Converters = TypedPrimitiveConverter.Default,
                        ValidatorType = typeof( DateTimeValidator ),
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

        [TypedPrimitive(typeof( System.DateTime ),
                        Converters = TypedPrimitiveConverter.None,
                        ValidatorType = typeof( DateTimeValidator ))]
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


        [TypedPrimitive(typeof( System.DateTime ), Converters = TypedPrimitiveConverter.None)]
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

        [TypedPrimitive(typeof( System.DateTime ),
                        Converters = TypedPrimitiveConverter.None,
                        ValidatorType = typeof( DateTimeValidator ),
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

        [TypedPrimitive(typeof( System.DateTime ),
                        Converters = TypedPrimitiveConverter.None,
                        ValidatorType = typeof( DateTimeValidator ),
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

        [TypedPrimitive(typeof( System.DateTime ),
                        ValidatorType = typeof( DateTimeValidator ))]
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


        [TypedPrimitive(typeof( System.DateTime ))]
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

        [TypedPrimitive(typeof( System.DateTime ),
                        ValidatorType = typeof( DateTimeValidator ),
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

        [TypedPrimitive(typeof( System.DateTime ),
                        ValidatorType = typeof( DateTimeValidator ),
                        ValidatorFlagsType = typeof( ValidatorFlags ))]
        public readonly partial record struct Test;
      """;

    return SourceGeneratorTestHelper.Verify<TypedPrimitiveSourceGenerator>( source, output );
  }

  #endregion
}
