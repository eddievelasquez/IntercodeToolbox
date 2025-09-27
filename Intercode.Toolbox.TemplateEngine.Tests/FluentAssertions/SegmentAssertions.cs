// Module Name: SegmentAssertions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

#pragma warning disable IDE0130
namespace FluentAssertions;
#pragma warning restore IDE0130
using FluentAssertions.Execution;
using Intercode.Toolbox.TemplateEngine;

/// <summary>
///   Provides fluent assertions for <see cref="Segment" />.
/// </summary>
internal sealed class SegmentAssertions( Segment Segment )
{
  #region Public Methods

  /// <summary>
  ///   Asserts that the segment is a constant segment, optionally matching its text and start.
  /// </summary>
  public AndConstraint<SegmentAssertions> BeConstant(
    string? text = null,
    string because = "",
    params object[] becauseArgs )
  {
    using var _ = new AssertionScope();

    Execute.Assertion
           .ForCondition( !Segment.IsMacro )
           .BecauseOf( because, becauseArgs )
           .FailWith(
             "Expected {context:segment} to be a constant segment{reason}, but it was a macro (name: {0}, slot: {1}).",
             Segment.Text,
             Segment.Slot
           );

    if( text is not null )
    {
      Execute.Assertion
             .ForCondition( Segment.Text == text )
             .BecauseOf( because, becauseArgs )
             .FailWith(
               "Expected {context:segment} constant text to be {0}{reason}, but found {1}.",
               text,
               Segment.Text
             );
    }

    return new AndConstraint<SegmentAssertions>( this );
  }

  /// <summary>
  ///   Asserts that the segment is a macro segment, optionally matching its name, argument and slot.
  /// </summary>
  public AndConstraint<SegmentAssertions> BeMacro(
    string? name = null,
    string? argument = null,
    string because = "",
    params object[] becauseArgs )
  {
    using var _ = new AssertionScope();

    Execute.Assertion
           .ForCondition( Segment.IsMacro )
           .BecauseOf( because, becauseArgs )
           .FailWith(
             "Expected {context:segment} to be a macro segment{reason}, but it was a constant (text: {0}).",
             Segment.Text
           );

    if( name is not null )
    {
      Execute.Assertion
             .ForCondition( Segment.Text == name )
             .BecauseOf( because, becauseArgs )
             .FailWith(
               "Expected {context:segment} macro name to be {0}{reason}, but found {1}.",
               name,
               Segment.Text
             );
    }

    if( argument is not null )
    {
      var actualArg = Segment.ArgumentMemory.ToString();

      Execute.Assertion
             .ForCondition( actualArg == argument )
             .BecauseOf( because, becauseArgs )
             .FailWith(
               "Expected {context:segment} macro argument to be {0}{reason}, but found {1}.",
               argument,
               actualArg
             );
    }

    return new AndConstraint<SegmentAssertions>( this );
  }

  /// <summary>
  ///   Asserts the segment text.
  /// </summary>
  public AndConstraint<SegmentAssertions> HaveText(
    string expected,
    string because = "",
    params object[] becauseArgs )
  {
    Execute.Assertion
           .ForCondition( Segment.Text == expected )
           .BecauseOf( because, becauseArgs )
           .FailWith(
             "Expected {context:segment} text to be {0}{reason}, but found {1}.",
             expected,
             Segment.Text
           );

    return new AndConstraint<SegmentAssertions>( this );
  }

  /// <summary>
  ///   Asserts the segment argument text.
  /// </summary>
  public AndConstraint<SegmentAssertions> HaveArgument(
    string expected,
    string because = "",
    params object[] becauseArgs )
  {
    var actual = Segment.ArgumentMemory.ToString();

    Execute.Assertion
           .ForCondition( actual == expected )
           .BecauseOf( because, becauseArgs )
           .FailWith(
             "Expected {context:segment} argument to be {0}{reason}, but found {1}.",
             expected,
             actual
           );

    return new AndConstraint<SegmentAssertions>( this );
  }

  /// <summary>
  ///   Asserts the segment slot value.
  /// </summary>
  public AndConstraint<SegmentAssertions> HaveSlot(
    int expected,
    string because = "",
    params object[] becauseArgs )
  {
    Execute.Assertion
           .ForCondition( Segment.Slot == expected )
           .BecauseOf( because, becauseArgs )
           .FailWith(
             "Expected {context:segment} slot to be {0}{reason}, but found {1}.",
             expected,
             Segment.Slot
           );

    return new AndConstraint<SegmentAssertions>( this );
  }

  #endregion
}
