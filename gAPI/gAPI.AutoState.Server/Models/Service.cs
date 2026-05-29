using Microsoft.CodeAnalysis;

namespace gAPI.AutoState.Server.Models;

public class Service : SharedReference
{
    public Service(Interface @interface, INamedTypeSymbol a) : base(a)
    {
    }
}