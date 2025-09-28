// Module Name: SegmentAssertions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests.FluentAssertions;

using global::FluentAssertions.Execution;
using global::FluentAssertions.Primitives;

/// <summary>
///   Provides fluent assertions for <see cref="Segment" />.
/// </summary>
internal sealed class SegmentAssertions: ObjectAssertions<Segment, SegmentAssertions>
{
  #region Constructors

  /// <inheritdoc />
  public SegmentAssertions(
    Template template,
    Segment segment )
    : base( segment )
  {
    Template = template;
  }

  #endregion

  #region Properties

  public Template Template { get; }

  #endregion

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
           .ForCondition( !Subject.IsMacro )
           .BecauseOf( because, becauseArgs )
           .FailWith(
             "Expected {context:segment} to be a constant segment{reason}, but it was a macro (name: {0}, slot: {1}).",
             Subject.GetText( Template ),
             Subject.Slot
           );

    if( text is not null )
    {
      Execute.Assertion
             .ForCondition( Subject.GetText( Template ) == text )
             .BecauseOf( because, becauseArgs )
             .FailWith(
               "Expected {context:segment} constant text to be {0}{reason}, but found {1}.",
               text,
               Subject.GetText( Template )
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
           .ForCondition( Subject.IsMacro )
           .BecauseOf( because, becauseArgs )
           .FailWith(
             "Expected {context:segment} to be a macro segment{reason}, but it was a constant (text: {0}).",
             Subject.GetText( Template )
           );

    if( name is not null )
    {
      Execute.Assertion
             .ForCondition( Subject.GetText( Template ) == name )
             .BecauseOf( because, becauseArgs )
             .FailWith(
               "Expected {context:segment} macro name to be {0}{reason}, but found {1}.",
               name,
               Subject.GetText( Template )
             );
    }

    if( argument is not null )
    {
      var actualArg = Subject.GetArgumentSpan( Template ).ToString();

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
           .ForCondition( Subject.GetText( Template ) == expected )
           .BecauseOf( because, becauseArgs )
           .FailWith(
             "Expected {context:segment} text to be {0}{reason}, but found {1}.",
             expected,
             Subject.GetText( Template )
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
    var actual = Subject.GetArgumentSpan( Template ).ToString();

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
           .ForCondition( Subject.Slot == expected )
           .BecauseOf( because, becauseArgs )
           .FailWith(
             "Expected {context:segment} slot to be {0}{reason}, but found {1}.",
             expected,
             Subject.Slot
           );

    return new AndConstraint<SegmentAssertions>( this );
  }

  #endregion
}
