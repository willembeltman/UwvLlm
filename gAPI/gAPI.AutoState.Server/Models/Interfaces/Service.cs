using Microsoft.CodeAnalysis;

namespace gAPI.AutoState.Server.Models.Interfaces;

public class Service : SharedReference
{
    public Service(Interface @interface, INamedTypeSymbol a) : base(a)
    {
    }
}