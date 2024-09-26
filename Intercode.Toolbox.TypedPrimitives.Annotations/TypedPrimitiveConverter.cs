namespace Intercode.Toolbox.TypedPrimitives;

using System;

[Flags]
public enum TypedPrimitiveConverter
{
  None = 0,
  TypeConverter = 1,
  SystemTextJson = 2,
  EfCoreValueConverter = 4,
  NewtonsoftJson = 8,
  Default = TypeConverter | SystemTextJson | EfCoreValueConverter
}
