// Module Name: ValueConverterTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests.Fixtures;

using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public abstract class ValueConverterTests<T, TValueConverter, TDataFactory>
  where TValueConverter: ValueConverter, new()
  where TDataFactory: ITestDataFactory
{
  #region Nested Types

  protected class Entity
  {
    #region Properties

    public Guid Id { get; set; } = Guid.NewGuid();
    public T? Value { get; set; }

    #endregion
  }

  protected class TestDbContext: DbContext
  {
    #region Properties

    public DbSet<Entity> Entities { get; set; } = null!;

    #endregion

    #region Implementation

    protected override void OnModelCreating(
      ModelBuilder modelBuilder )
    {
      modelBuilder.Entity<Entity>()
                  .Property( e => e.Value )
                  .HasConversion( new TValueConverter() )
                  .ValueGeneratedNever();
    }

    protected override void OnConfiguring(
      DbContextOptionsBuilder optionsBuilder )
    {
      var connection = new SqliteConnection( "DataSource=:memory:" );
      connection.Open();

      optionsBuilder.UseSqlite( connection );
    }

    #endregion
  }

  #endregion

  #region Public Methods

  [Theory]
  [MemberData( nameof( GetData ) )]
  public void ValueConverter_ShouldRoundTrip(
    T? value )
  {
    // Arrange
    using var context = new TestDbContext();
    context.Database.EnsureCreated();

    var id = Guid.NewGuid();

    var entity = new Entity
    {
      Id = id,
      Value = value
    };

    // Act
    context.Entities.Add( entity );
    context.SaveChanges();

    // Assert
    var result = context.Entities.Single( e => e.Id == id );

    result.Value.Should().Be( value );
  }

  public static IEnumerable<object?[]> GetData()
  {
    return TDataFactory.GetValidValues();
  }

  #endregion
}
