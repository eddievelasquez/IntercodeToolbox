// Module Name: IncrementalValuesProviderExtensions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using Microsoft.CodeAnalysis;

internal static class IncrementalValuesProviderExtensions
{
  #region Public Methods

  public static IncrementalValuesProvider<T> Merge<T>(
    this IncrementalValuesProvider<T> first,
    IncrementalValuesProvider<T> second )
  {
    var merged = first.Collect()
                      .Combine( second.Collect() )
                      .SelectMany(
                        (
                          tuple,
                          _ ) => tuple.Left.Concat( tuple.Right )
                      );
    return merged;
  }

  public static IncrementalValueProvider<bool> Any<T>(
    this IncrementalValuesProvider<T> provider,
    Func<T, bool> predicate )
  {
    var any = provider.Where( predicate )
                      .Select(
                        (
                          _,
                          _ ) => true
                      )
                      .Collect()
                      .Select(
                        (
                          values,
                          _ ) => values.Any()
                      );
    return any;
  }

  #endregion
}
