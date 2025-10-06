// Module Name: Result.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System.Diagnostics;
using Intercode.Toolbox.TypedPrimitives.Diagnostics;

// FluentResults-inspired result type that works well
// in a source generator

internal abstract class ResultBase
{
  #region Constructors

  protected ResultBase(
    EquatableArray<DiagnosticInfo>? errors )
  {
    Errors = errors ?? EquatableArray<DiagnosticInfo>.Empty;
  }

  #endregion

  #region Properties

  public EquatableArray<DiagnosticInfo> Errors { get; }

  public bool IsFailed => Errors.Length > 0;
  public bool IsSuccess => !IsFailed;

  #endregion
}

internal abstract class ResultBase<TResult>: ResultBase
  where TResult: ResultBase<TResult>
{
  #region Constructors

  protected ResultBase(
    EquatableArray<DiagnosticInfo>? errors )
    : base( errors )
  {
  }

  #endregion
}

[DebuggerDisplay( "{GetDebuggerDisplay() ,nq}" )]
internal class Result<T>: ResultBase<Result<T>>
{
  #region Constructors

  public Result(
    EquatableArray<DiagnosticInfo>? errors )
    : base( errors )
  {
    Value = default;
  }

  public Result(
    T value )
    : base( null )
  {
    Value = value;
  }

  #endregion

  #region Properties

  public T? Value { get; }

  #endregion

  #region Implementation

  private string GetDebuggerDisplay()
  {
    return IsSuccess ? $"Value = {Value}" : $"Errors = {Errors}";
  }

  #endregion
}

internal class Result: ResultBase<Result>
{
  #region Constructors

  private Result(
    EquatableArray<DiagnosticInfo>? errors )
    : base( errors )
  {
  }

  #endregion

  #region Public Methods

  public static Result Ok()
  {
    return new Result( null );
  }

  public static Result Fail(
    DiagnosticInfo diagnosticInfo )
  {
    return new Result( new EquatableArray<DiagnosticInfo>( [diagnosticInfo] ) );
  }

  public static Result Fail(
    EquatableArray<DiagnosticInfo>? errors = null )
  {
    return new Result( errors );
  }

  public static Result Fail(
    IEnumerable<DiagnosticInfo> errors )
  {
    return new Result( new EquatableArray<DiagnosticInfo>( errors.ToArray() ) );
  }

  public static Result<T> Ok<T>(
    T value )
  {
    return new Result<T>( value );
  }

  public static Result<T> Fail<T>(
    DiagnosticInfo diagnosticInfo )
  {
    return new Result<T>( new EquatableArray<DiagnosticInfo>( [diagnosticInfo] ) );
  }

  public static Result<T> Fail<T>(
    EquatableArray<DiagnosticInfo>? errors = null )
  {
    return new Result<T>( errors );
  }

  public static Result<T> Fail<T>(
    IEnumerable<DiagnosticInfo> errors )
  {
    return new Result<T>( new EquatableArray<DiagnosticInfo>( errors.ToArray() ) );
  }

  #endregion
}
