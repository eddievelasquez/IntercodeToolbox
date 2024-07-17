# Intercode.Toolbox
The Intercode Toolbox is a collection of libraries that I have developed throught the years, recently, I started unifying the into a unified set.
Althought I have tried to make them as general purpose as possible, they are still modelled to the the particular needs of the projects they were created for; 
this means the might not be ideal for your needs. I plan to make them more generic and open to PR contributions in the future.

# Libraries
There are currently three libraries that are part of the Intercode Toolbox:

- `Intercode.Toolbox.Core`: A trimmable, AOT-compatible .NET library that provides general purpose functionality, such as integer variable length encoding, string and stream extensions, and more.

- `Intercode.Toolbox.Collections`: A trimmable, AOT-compatible .NET library that provides classes that define generic collections and collection-related utilities, such as
  `MutableListLookup`, `MutableHashSetLookup`, and more.

- `Intercode.Toolbox.AspNetCore.Extensions`: A trimmable, AOT-compatible .NET library that contains types that provide functionality commonly used in ASP.NET Core applications.
It currently exposes the `JsonWebTokenBuilder` and `OpenApiInfoBuilder` classes.

There are several additional libraries that are currently being cleaned up and will be added to the toolbox in the future.
