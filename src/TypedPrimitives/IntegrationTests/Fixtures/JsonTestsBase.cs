// Module Name: JsonTestsBase.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests.Fixtures;

public abstract class JsonTestsBase<T, TDataFactory>
  where TDataFactory: ITestDataFactory
{
  #region Nested Types

  protected class Test
  {
    #region Properties

    public T? Value { get; set; }

    #endregion
  }

  #endregion

  #region Public Methods

  public static string ToJson(
    object? value )
  {
    return $$"""{"Value":{{value.ToStringForJson()}}}""";
  }

  public static IEnumerable<object?[]> GetData()
  {
    return TDataFactory.GetValidValues();
  }

  public static IEnumerable<object?[]> GetInvalidData()
  {
    return TDataFactory.GetInvalidValues();
  }

  #endregion
}
