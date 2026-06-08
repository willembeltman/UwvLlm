using gAPI.AutoState.Server.Models.Interfaces;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace gAPI.AutoState.Server.Models.Entities;

public class Entity : SharedReference
{
    public Entity(ServiceContext serviceContext, INamedTypeSymbol symbol, INamedTypeSymbol[] allSymbols)
    {
        ServiceContext = serviceContext;
        Symbol = symbol;
        AllSymbols = allSymbols;
    }

    public EntityKey KeyProperty { get; } = new();
    public ServiceContext ServiceContext { get; }
    public INamedTypeSymbol Symbol { get; }
    public INamedTypeSymbol[] AllSymbols { get; }
}
