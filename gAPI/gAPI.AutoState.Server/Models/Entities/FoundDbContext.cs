using gAPI.AutoState.Server.Models.Interfaces;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace gAPI.AutoState.Server.Models.Entities;

public class FoundDbContext
{
    public FoundDbContext(ServiceContext serviceContext, INamedTypeSymbol symbol, INamedTypeSymbol[] allSymbols)
    {
        ServiceContext = serviceContext;
        Symbol = symbol;
        AllSymbols = allSymbols;
    }

    public ServiceContext ServiceContext { get; }
    public INamedTypeSymbol Symbol { get; }
    public INamedTypeSymbol[] AllSymbols { get; }
}
