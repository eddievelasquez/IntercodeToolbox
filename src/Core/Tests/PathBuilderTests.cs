// Module Name: PathBuilderTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Core.Tests;

using FluentAssertions;
using Intercode.Toolbox.Core.IO;

public class PathBuilderTests
{
  #region Tests

  [Fact]
  public void AppendDirectories_ShouldAppendDirectories()
  {
    var builder = new PathBuilder( @"C:\Temp\file.txt" );
    var result = builder.AppendDirectories( "SubDirectory1", "SubDirectory2" );

    result.Should()
          .BeSameAs( builder );

    builder.Build()
           .Should()
           .Be( @"C:\Temp\SubDirectory1\SubDirectory2\file.txt" );
  }

  [Fact]
  public void AppendDirectory_ShouldAppendDirectory()
  {
    var builder = new PathBuilder( @"C:\Temp\file.txt" );
    var result = builder.AppendDirectory( "SubDirectory" );

    result.Should()
          .BeSameAs( builder );

    builder.Build()
           .Should()
           .Be( @"C:\Temp\SubDirectory\file.txt" );
  }

  [Fact]
  public void ChangeDirectory_ShouldChangeDirectory()
  {
    var builder = new PathBuilder( @"C:\Temp\file.txt" );
    var result = builder.ChangeDirectory( dir => dir + "-new" );

    result.Should()
          .BeSameAs( builder );

    builder.Build()
           .Should()
           .Be( @"C:\Temp-new\file.txt" );
  }

  [Fact]
  public void ChangeExtension_ShouldChangeExtension()
  {
    var builder = new PathBuilder( @"C:\Temp\file.txt" );
    var result = builder.ChangeExtension( extension => extension?.Replace( 'x', 'y' ) );

    result.Should()
          .BeSameAs( builder );

    builder.Build()
           .Should()
           .Be( @"C:\Temp\file.tyt" );
  }

  [Fact]
  public void ChangeFilename_ShouldChangeFilename()
  {
    var builder = new PathBuilder( @"C:\Temp\file.txt" );
    var result = builder.ChangeFilename( filename => "new-" + filename );

    result.Should()
          .BeSameAs( builder );
    builder.Build()
           .Should()
           .Be( @"C:\Temp\new-file.txt" );
  }

  [Fact]
  public void DefaultConstructor_ShouldSucceed()
  {
    var builder = new PathBuilder();
    var result = builder.Build();
    result.Should()
          .BeEmpty();
  }

  [Fact]
  public void Constructor_WithDirectoryInfo_ShouldSucceed()
  {
    var builder = new PathBuilder( new DirectoryInfo( @"C:\Temp" ) );
    var result = builder.Build();
    result.Should()
          .Be( @"C:\Temp" );
  }

  [Fact]
  public void Constructor_WithFileInfo_ShouldSucceed()
  {
    var builder = new PathBuilder( new FileInfo( @"C:\Temp\file.txt" ) );
    var result = builder.Build();
    result.Should()
          .Be( @"C:\Temp\file.txt" );
  }

  [Fact]
  public void Constructor_WithString_ShouldSucceed()
  {
    var builder = new PathBuilder( @"C:\Temp\file.txt" );
    var result = builder.Build();
    result.Should()
          .Be( @"C:\Temp\file.txt" );
  }

  [Fact]
  public void SetDirectory_ShouldSetDirectory()
  {
    var builder = new PathBuilder( @"C:\Temp\file.txt" );
    var result = builder.SetDirectory( @"C:\NewDirectory" );

    result.Should()
          .BeSameAs( builder );
    builder.Build()
           .Should()
           .Be( @"C:\NewDirectory\file.txt" );
  }

  [Fact]
  public void SetDirectory_WithDirectoryInfo_ShouldSetDirectory()
  {
    var builder = new PathBuilder( @"C:\Temp\file.txt" );
    var directoryInfo = new DirectoryInfo( @"C:\NewDirectory" );
    var result = builder.SetDirectory( directoryInfo );

    result.Should()
          .BeSameAs( builder );

    builder.Build()
           .Should()
           .Be( @"C:\NewDirectory\file.txt" );
  }

  [Fact]
  public void SetExtension_ShouldSetExtension()
  {
    var builder = new PathBuilder( @"C:\Temp\file.txt" );
    var result = builder.SetExtension( ".doc" );

    result.Should()
          .BeSameAs( builder );

    builder.Build()
           .Should()
           .Be( @"C:\Temp\file.doc" );
  }

  [Fact]
  public void SetFilename_ShouldSetFilename()
  {
    var builder = new PathBuilder( @"C:\Temp\file.txt" );
    var result = builder.SetFilename( "new" );

    result.Should()
          .BeSameAs( builder );

    builder.Build()
           .Should()
           .Be( @"C:\Temp\new.txt" );
  }

  #endregion
}
