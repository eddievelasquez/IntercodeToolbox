# Intercode.Toolbox
The Intercode Toolbox is a collection of libraries that I have developed over the years; recently, I started collecting them in this unified tool set.
Although I have tried to make them as general purpose as possible, they are still modeled to the particular needs of the projects they were created for; 
this means it might not be ideal for your needs. I plan to make them more generic and open to PR contributions.

# Libraries
These are libraries that have been ported into the Intercode Toolbox:

- `Intercode.Toolbox.Core`: A trimmable, AOT-compatible .NET library that provides general-purpose functionality, such as integer variable length encoding, string and stream extensions, and more. Download the NuGet package [here](https://www.nuget.org/packages/Intercode.Toolbox.Core/).

- `Intercode.Toolbox.Collections`: A trimmable, AOT-compatible .NET library that provides classes that define generic collections and collection-related utilities, such as
  `MutableListLookup`, `MutableHashSetLookup`, and more.  Download the NuGet package [here](https://www.nuget.org/packages/Intercode.Toolbox.Collections/).

- `Intercode.Toolbox.AspNetCore.Extensions`: A trimmable, AOT-compatible .NET library that contains types that provide functionality commonly used in ASP.NET Core applications.
It currently exposes the `JsonWebTokenBuilder` and `OpenApiInfoBuilder` classes.  Download the NuGet package [here](https://www.nuget.org/packages/Intercode.Toolbox.AspNetCore.Extensions/).

- `Intercode.Toolbox.UnitTesting`: A trimmable, AOT-compatible .NET library that provides tools for unit testing.  Download the NuGet package [here](https://www.nuget.org/packages/Intercode.Toolbox.UnitTesting/).

- `Intercode.Toolbox.UnitTesting.XUnit`: A trimmable, AOT-compatible .NET library that provides logging and
dependency injection tools for unit testing using the [XUnit](https://xunit.net/) library.  Download the NuGet package [here](https://www.nuget.org/packages/Intercode.Toolbox.UnitTesting.XUnit/).

Several additional libraries are currently being cleaned up and will be added to the toolbox in the future.
