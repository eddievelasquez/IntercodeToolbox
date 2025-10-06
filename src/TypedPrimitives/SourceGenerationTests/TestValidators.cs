// Module Name: TestValidators.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

using FluentResults;

public static class StringValidator
{
  #region Public Methods

  public static Result Validate(
    string? value )
  {
    return Result.Ok();
  }

  public static Result Validate(
    ReadOnlySpan<char> span )
  {
    return Result.Ok();
  }

  #endregion
}

public static class IntValidator
{
  #region Public Methods

  public static Result Validate(
    int? value )
  {
    return Result.Ok();
  }

  #endregion
}

public static class LongValidator
{
  #region Public Methods

  public static Result Validate(
    long? value )
  {
    return Result.Ok();
  }

  #endregion
}

public static class GuidValidator
{
  #region Public Methods

  public static Result Validate(
    Guid? value )
  {
    return Result.Ok();
  }

  #endregion
}

public static class DateTimeValidator
{
  #region Public Methods

  public static Result Validate(
    DateTime? value )
  {
    return Result.Ok();
  }

  #endregion
}

public static class DateTimeOffsetValidator
{
  #region Public Methods

  public static Result Validate(
    DateTimeOffset? value )
  {
    return Result.Ok();
  }

  #endregion
}
