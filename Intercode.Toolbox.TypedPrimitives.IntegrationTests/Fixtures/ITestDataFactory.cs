// Module Name: ITestDataFactory.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests.Fixtures;

public interface ITestDataFactory
{
  public static abstract IEnumerable<object?[]> GetValidValues();
  public static abstract IEnumerable<object?[]> GetInvalidValues();
}
