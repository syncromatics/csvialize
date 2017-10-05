# Csvialize

Adds CSV support to [Nancy](http://nancyfx.org/) using [CsvHelper](https://joshclose.github.io/CsvHelper/)

## Quickstart

Add the [csvialize](https://www.nuget.org/packages/csvialize/) NuGet package to your project in the manner applicable to you.

If you're using Nancy's auto-registration, there should be no additional work to do. Requests with an `Accept: text/csv` header will be negotiated as CSV output.

It may be desirable to change the order of the default `IResponseProcessor` so that requests without an `Accept` header are negotiated as desired. For example, this will ensure that Csvialize is the default response processor.

```csharp
public class CsvFirstBootstrapper : DefaultNancyBootstrapper
{
    protected override NancyInternalConfiguration InternalConfiguration => NancyInternalConfiguration.WithOverrides(c =>
    {
        // Remove Csvialize if it exists
        c.ResponseProcessors.Remove(typeof(Csvialize.CsvProcessor));

        // Insert it at the start of the list
        c.ResponseProcessors.Insert(0, typeof(Csvialize.CsvProcessor));
    });
}

```

See [The response processors](https://github.com/NancyFx/Nancy/wiki/Content-Negotiation#the-response-processors) in the Nancy wiki for more information.

## Building

[![AppVeyor](https://img.shields.io/appveyor/ci/derrickcrowne/csvialize.svg)](https://ci.appveyor.com/project/derrickcrowne/csvialize)
[![NuGet](https://img.shields.io/nuget/v/csvialize.svg)](https://www.nuget.org/packages/csvialize/)
[![NuGet Pre Release](https://img.shields.io/nuget/vpre/csvialize.svg)](https://www.nuget.org/packages/csvialize/)

This library is built with [.NET Core](https://www.microsoft.com/net/core)

To build the project:

```bash
dotnet restore
dotnet build
```

To run the tests:

```bash
dotnet test
```

## Code of Conduct

We are committed to fostering an open and welcoming environment. Please read our [code of conduct](CODE_OF_CONDUCT.md) before participating in or contributing to this project.

## Contributing

We welcome contributions and collaboration on this project. Please read our [contributor's guide](CONTRIBUTING.md) to understand how best to work with us.

## License and Authors

[![Syncromatics Engineering logo](https://en.gravatar.com/userimage/100017782/89bdc96d68ad4b23998e3cdabdeb6e13.png?size=16) Syncromatics Engineering](https://github.com/syncromatics)

![license](https://img.shields.io/github/license/syncromatics/csvialize.svg)
![GitHub contributors](https://img.shields.io/github/contributors/syncromatics/csvialize.svg)

This software is made available by Syncromatics Engineering under the MIT license.