namespace Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

using System;
using FluentResults;

public enum ValidatorFlags
{
  None,
  Full
}

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

  public static Result Validate(
    string? value,
    ValidatorFlags flags )
  {
    return Result.Ok();
  }

  public static Result Validate(
    ReadOnlySpan<char> span,
    ValidatorFlags flags )
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

  public static Result Validate(
    int? value,
    ValidatorFlags flags )
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

  public static Result Validate(
    long? value,
    ValidatorFlags flags )
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

  public static Result Validate(
    Guid? value,
    ValidatorFlags flags )
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

  public static Result Validate(
    DateTime? value,
    ValidatorFlags flags )
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

  public static Result Validate(
    DateTimeOffset? value,
    ValidatorFlags flags )
  {
    return Result.Ok();
  }

  #endregion
}
