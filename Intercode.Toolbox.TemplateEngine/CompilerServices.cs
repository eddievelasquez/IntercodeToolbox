// Module Name: CompilerServices.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

// ReSharper disable once CheckNamespace

namespace System.Runtime.CompilerServices
{
  using System.ComponentModel;

  // Types missing in netstandard2.0 required by newer C# features

  [EditorBrowsable( EditorBrowsableState.Never )]
  internal class IsExternalInit
  {
  }
}

// https://github.com/dotnet/runtime/blob/527f9ae88a0ee216b44d556f9bdc84037fe0ebda/src/libraries/System.Private.CoreLib/src/System/Diagnostics/CodeAnalysis/NullableAttributes.cs
#pragma warning disable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
namespace System.Diagnostics.CodeAnalysis
{
#if NETSTANDARD2_0 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NET45 || NET451 || NET452 || NET46 || NET461 || NET462 || NET47 || NET471 || NET472 || NET48
  /// <summary>Specifies that null is allowed as an input even if the corresponding type disallows it.</summary>
  [AttributeUsage( AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property )]
  internal sealed class AllowNullAttribute: Attribute
  {
  }

  /// <summary>Specifies that null is disallowed as an input even if the corresponding type allows it.</summary>
  [AttributeUsage( AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property )]
  internal sealed class DisallowNullAttribute: Attribute
  {
  }

  /// <summary>Specifies that an output may be null even if the corresponding type disallows it.</summary>
  [AttributeUsage(
    AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue
  )]
  internal sealed class MaybeNullAttribute: Attribute
  {
  }

  /// <summary>
  ///   Specifies that an output will not be null even if the corresponding type allows it. Specifies that an input
  ///   argument was not null when the call returns.
  /// </summary>
  [AttributeUsage(
    AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue
  )]
  internal sealed class NotNullAttribute: Attribute
  {
  }

  /// <summary>
  ///   Specifies that when a method returns <see cref="ReturnValue" />, the parameter may be null even if the
  ///   corresponding type disallows it.
  /// </summary>
  [AttributeUsage( AttributeTargets.Parameter )]
  internal sealed class MaybeNullWhenAttribute: Attribute
  {
    #region Constructors

    /// <summary>Initializes the attribute with the specified return value condition.</summary>
    /// <param name="returnValue">
    ///   The return value condition. If the method returns this value, the associated parameter may be null.
    /// </param>
    public MaybeNullWhenAttribute(
      bool returnValue )
    {
      ReturnValue = returnValue;
    }

    #endregion

    #region Properties

    /// <summary>Gets the return value condition.</summary>
    public bool ReturnValue { get; }

    #endregion
  }

  /// <summary>
  ///   Specifies that when a method returns <see cref="ReturnValue" />, the parameter will not be null even if the
  ///   corresponding type allows it.
  /// </summary>
  [AttributeUsage( AttributeTargets.Parameter )]
  internal sealed class NotNullWhenAttribute: Attribute
  {
    #region Constructors

    /// <summary>Initializes the attribute with the specified return value condition.</summary>
    /// <param name="returnValue">
    ///   The return value condition. If the method returns this value, the associated parameter will not be null.
    /// </param>
    public NotNullWhenAttribute(
      bool returnValue )
    {
      ReturnValue = returnValue;
    }

    #endregion

    #region Properties

    /// <summary>Gets the return value condition.</summary>
    public bool ReturnValue { get; }

    #endregion
  }

  /// <summary>Specifies that the output will be non-null if the named parameter is non-null.</summary>
  [AttributeUsage(
    AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue,
    AllowMultiple = true
  )]
  internal sealed class NotNullIfNotNullAttribute: Attribute
  {
    #region Constructors

    /// <summary>Initializes the attribute with the associated parameter name.</summary>
    /// <param name="parameterName">
    ///   The associated parameter name.  The output will be non-null if the argument to the parameter specified is non-null.
    /// </param>
    public NotNullIfNotNullAttribute(
      string parameterName )
    {
      ParameterName = parameterName;
    }

    #endregion

    #region Properties

    /// <summary>Gets the associated parameter name.</summary>
    public string ParameterName { get; }

    #endregion
  }

  /// <summary>Applied to a method that will never return under any circumstance.</summary>
  [AttributeUsage( AttributeTargets.Method, Inherited = false )]
  internal sealed class DoesNotReturnAttribute: Attribute
  {
  }

  /// <summary>Specifies that the method will not return if the associated Boolean parameter is passed the specified value.</summary>
  [AttributeUsage( AttributeTargets.Parameter )]
  internal
    sealed class DoesNotReturnIfAttribute: Attribute
  {
    #region Constructors

    /// <summary>Initializes the attribute with the specified parameter value.</summary>
    /// <param name="parameterValue">
    ///   The condition parameter value. Code after the method will be considered unreachable by diagnostics if the argument to
    ///   the associated parameter matches this value.
    /// </param>
    public DoesNotReturnIfAttribute(
      bool parameterValue )
    {
      ParameterValue = parameterValue;
    }

    #endregion

    #region Properties

    /// <summary>Gets the condition parameter value.</summary>
    public bool ParameterValue { get; }

    #endregion
  }
#endif

#if NETSTANDARD2_0 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETCOREAPP3_0 || NETCOREAPP3_1 || NET45 || NET451 || NET452 || NET46 || NET461 || NET462 || NET47 || NET471 || NET472 || NET48
  /// <summary>
  ///   Specifies that the method or property will ensure that the listed field and property members have not-null
  ///   values.
  /// </summary>
  [AttributeUsage( AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true )]
  internal sealed class MemberNotNullAttribute: Attribute
  {
    #region Constructors

    /// <summary>Initializes the attribute with a field or property member.</summary>
    /// <param name="member">
    ///   The field or property member that is promised to be not-null.
    /// </param>
    public MemberNotNullAttribute(
      string member )
    {
      Members = new[] { member };
    }

    /// <summary>Initializes the attribute with the list of field and property members.</summary>
    /// <param name="members">
    ///   The list of field and property members that are promised to be not-null.
    /// </param>
    public MemberNotNullAttribute(
      params string[] members )
    {
      Members = members;
    }

    #endregion

    #region Properties

    /// <summary>Gets field or property member names.</summary>
    public string[] Members { get; }

    #endregion
  }

  /// <summary>
  ///   Specifies that the method or property will ensure that the listed field and property members have not-null
  ///   values when returning with the specified return value condition.
  /// </summary>
  [AttributeUsage( AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true )]
  internal sealed class MemberNotNullWhenAttribute: Attribute
  {
    #region Constructors

    /// <summary>Initializes the attribute with the specified return value condition and a field or property member.</summary>
    /// <param name="returnValue">
    ///   The return value condition. If the method returns this value, the associated parameter will not be null.
    /// </param>
    /// <param name="member">
    ///   The field or property member that is promised to be not-null.
    /// </param>
    public MemberNotNullWhenAttribute(
      bool returnValue,
      string member )
    {
      ReturnValue = returnValue;
      Members = new[] { member };
    }

    /// <summary>Initializes the attribute with the specified return value condition and list of field and property members.</summary>
    /// <param name="returnValue">
    ///   The return value condition. If the method returns this value, the associated parameter will not be null.
    /// </param>
    /// <param name="members">
    ///   The list of field and property members that are promised to be not-null.
    /// </param>
    public MemberNotNullWhenAttribute(
      bool returnValue,
      params string[] members )
    {
      ReturnValue = returnValue;
      Members = members;
    }

    #endregion

    #region Properties

    /// <summary>Gets the return value condition.</summary>
    public bool ReturnValue { get; }

    /// <summary>Gets field or property member names.</summary>
    public string[] Members { get; }

    #endregion
  }
#endif
}
