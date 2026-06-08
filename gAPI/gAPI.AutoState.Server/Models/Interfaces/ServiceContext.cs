using gAPI.AutoState.Server.Helpers;
using gAPI.AutoState.Server.Models.Entities;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gAPI.AutoState.Server.Models.Interfaces;


public class ServiceContext
{
    public ServiceContext(INamedTypeSymbol[] allSymbols)
    {
        var hubInterfaceSymbols = allSymbols
            .Where(t =>
                t.TypeKind == TypeKind.Interface &&
                t.HasAttribute("gAPI.Core.Attributes.GenerateHubAttribute"))
            .ToArray();

        HubInterfaces = hubInterfaceSymbols
            .Select(interfaceSymbol => new Interface(this, interfaceSymbol, allSymbols))
            .ToArray();

        var apiInterfaceSymbols = allSymbols
            .Where(t =>
                t.TypeKind == TypeKind.Interface &&
                t.HasAttribute("gAPI.Core.Attributes.GenerateApiAttribute"))
            .ToArray();

        ApiInterfaces = apiInterfaceSymbols
            .Select(interfaceSymbol => new Interface(this, interfaceSymbol, allSymbols))
            .ToArray();

        var minimalApiInterfaceSymbols = allSymbols
            .Where(t =>
                t.TypeKind == TypeKind.Interface &&
                t.HasAttribute("gAPI.Core.Attributes.GenerateMinimalApiAttribute"))
            .ToArray();

        MinimalApiInterfaces = minimalApiInterfaceSymbols
            .Select(interfaceSymbol => new Interface(this, interfaceSymbol, allSymbols))
            .ToArray();



        Found_DbContext = FindSingleDerived<FoundDbContext>(
            allSymbols,
            "AuthenticatedDbContext",
            symbol => new FoundDbContext(this, symbol, allSymbols));

        Found_UserEntity = FindSingleDerived<Entity>(
            allSymbols,
            "gAPI.Core.Server.Entities.AuthenticatedUser",
            symbol => new Entity(this, symbol, allSymbols));

        Found_StateUserMapper = FindSingleDerived<TypeHelper>(
            allSymbols,
            "gAPI.Core.Server.Mappers.AuthenticatedStateUserMapper",
            symbol => new TypeHelper(this, symbol, false));

        Found_StateUserDto = FindSingleDerived<TypeHelper>(
            allSymbols,
            "gAPI.Core.Dtos.AuthenticatedStateUserDto",
            symbol => new TypeHelper(this, symbol, false));

        Found_AuthenticationState = FindSingleDerived<TypeHelper>(
            allSymbols,
            "gAPI.Core.Server.Dtos.AuthenticationState",
            symbol => new TypeHelper(this, symbol, false));
    }
    public Interface[] HubInterfaces { get; }
    public Interface[] ApiInterfaces { get; }
    public Interface[] MinimalApiInterfaces { get; }

    public FoundDbContext? Found_DbContext { get; }

    public Entity? Found_UserEntity { get; }
    public TypeHelper? Found_StateUserMapper { get; }
    public TypeHelper? Found_StateUserDto { get; }

    public TypeHelper? Found_AuthenticationState { get; }
    public TypeHelper? Found_StateMapper { get; }
    public TypeHelper? Found_StateDto { get; }

    private T? FindSingleDerived<T>(
        IEnumerable<INamedTypeSymbol> allSymbols,
        string baseTypeName,
        Func<INamedTypeSymbol, T> create)
        where T : class
    {
        var matches = allSymbols
            .Where(t =>
                t.TypeKind == TypeKind.Class &&
                InheritsFrom(t, baseTypeName))
            .ToArray();

        if (matches.Length == 0)
            return null;

        var topMostMatches = matches
            .Where(candidate =>
                !matches.Any(other =>
                    !SymbolEqualityComparer.Default.Equals(candidate, other) &&
                    InheritsFrom(other, candidate)))
            .ToArray();

        if (topMostMatches.Length > 1)
        {
            throw new Exception(
                $"Multiple top-level implementations found for '{baseTypeName}': " +
                string.Join(", ", topMostMatches.Select(a => a.ToDisplayString())));
        }

        return create(topMostMatches[0]);
    }
    private bool InheritsFrom(INamedTypeSymbol symbol, string baseTypeName)
    {
        var current = symbol.BaseType;

        while (current != null)
        {
            if (current.ToDisplayString() == baseTypeName ||
                current.Name == baseTypeName)
            {
                return true;
            }

            current = current.BaseType;
        }

        return false;
    }
    private bool InheritsFrom(INamedTypeSymbol symbol, INamedTypeSymbol possibleBase)
    {
        var current = symbol.BaseType;

        while (current != null)
        {
            if (SymbolEqualityComparer.Default.Equals(current, possibleBase))
                return true;

            current = current.BaseType;
        }

        return false;
    }
    public List<string> CheckForErrors()
    {
        var errors = new List<string>();

        foreach (var hubInterface in HubInterfaces)
            foreach (var method in hubInterface.Methods)
                CheckHub(method.ResponseType, errors, method.Name, hubInterface.FullName);

        foreach (var hubInterface in ApiInterfaces)
            foreach (var method in hubInterface.Methods)
                CheckApi(method.ResponseType, errors, method.Name, hubInterface.FullName);

        foreach (var hubInterface in MinimalApiInterfaces)
            foreach (var method in hubInterface.Methods)
                CheckApi(method.ResponseType, errors, method.Name, hubInterface.FullName);

        return errors;
    }

    private void CheckHub(TypeHelper responseType, List<string> errors, string method, string hubInterface)
    {
        if (responseType.IsTaskT)
        {
            errors.Add(
                $"Method '{method}' on interface '{hubInterface}' returns Task<T>. " +
                "Please use IAsyncEnumerable<T> instead. " +
                "When communicating from server to client, the number of client responses is unknown, " +
                "therefore response methods must use IAsyncEnumerable<T>.");
        }
        else if (!responseType.IsTask && !responseType.IsIAsyncEnumerable)
        {
            errors.Add(
                $"Method '{method}' on interface '{hubInterface}' appears to be synchronous. " +
                "Please use Task (no response) or IAsyncEnumerable<T> (with responses).");
        }
    }

    private void CheckApi(TypeHelper responseType, List<string> errors, string method, string hubInterface)
    {
        if (!responseType.IsTaskT && !responseType.IsTask && !responseType.IsIAsyncEnumerable)
        {
            errors.Add(
                $"Method '{method}' on interface '{hubInterface}' appears to be synchronous. " +
                "Please use Task (no response), Task<T> or IAsyncEnumerable<T> (with responses).");
        }
    }
}

//public class ServiceContext
//{
//    public ServiceContext(INamedTypeSymbol[] allSymbols)
//    {
//        var hubInterfaceSymbols = allSymbols
//            .Where(t =>
//                t.TypeKind == TypeKind.Interface &&
//                t.HasAttribute("gAPI.Core.Attributes.GenerateHubAttribute"))
//            .ToArray();

//        HubInterfaces = hubInterfaceSymbols
//            .Select(interfaceSymbol => new Interface(this, interfaceSymbol, allSymbols))
//            .ToArray();

//        var apiInterfaceSymbols = allSymbols
//            .Where(t =>
//                t.TypeKind == TypeKind.Interface &&
//                t.HasAttribute("gAPI.Core.Attributes.GenerateApiAttribute"))
//            .ToArray();

//        ApiInterfaces = apiInterfaceSymbols
//            .Select(interfaceSymbol => new Interface(this, interfaceSymbol, allSymbols))
//            .ToArray();

//        var minimalApiInterfaceSymbols = allSymbols
//            .Where(t =>
//                t.TypeKind == TypeKind.Interface &&
//                t.HasAttribute("gAPI.Core.Attributes.GenerateMinimalApiAttribute"))
//            .ToArray();

//        MinimalApiInterfaces = minimalApiInterfaceSymbols
//            //.Where(a => a.ToDisplayString() != "gAPI.Core.Interfaces.IAccountService")
//            .Select(interfaceSymbol => new Interface(this, interfaceSymbol, allSymbols))
//            .ToArray();

//        // Found_DbContext = Bovenste overervende van "AuthenticatedDbContext" (Letop: geen namespace zoeken en er mag maar 1 pad zijn)
//        // Ik weet ook niet of het wel kan wat ik hierboven wil doen, deze analyzer gaat namelijk AuthenticatedDbContext genereren, die dan door de boven gevonden class gebruikt wordt

//        // Found_UserEntity = Bovenste overervende van "gAPI.Core.Server.Entities.AuthenticatedUser" (Letop: er mag maar 1 pad zijn)
//        // Found_StateUserMapper = Bovenste overervende van "gAPI.Core.Server.Mappers.AuthenticatedStateUserMapper" (Letop: er mag maar 1 pad zijn)
//        // Found_StateUserDto = Bovenste overervende van "gAPI.Core.Dtos.AuthenticatedStateUserDto" (Letop: er mag maar 1 pad zijn)

//        // Found_AuthenticationState = Bovenste overervende van "gAPI.Core.Server.Dtos.AuthenticationState" (Letop: er mag maar 1 pad zijn)
//        // Found_StateMapper = Bovenste overervende van "gAPI.Core.Server.Mappers.StateMapper" (Letop: er mag maar 1 pad zijn)
//        // Found_StateDto = Bovenste overervende van "gAPI.Core.Dtos.StateDto" (Letop: er mag maar 1 pad zijn)
//    }

//    public Interface[] HubInterfaces { get; }
//    public Interface[] ApiInterfaces { get; }
//    public Interface[] MinimalApiInterfaces { get; }

//    public FoundDbContext? Found_DbContext { get; }

//    public Entity? Found_UserEntity { get; }
//    public TypeHelper? Found_StateUserMapper { get; }
//    public TypeHelper? Found_StateUserDto { get; }

//    public TypeHelper? Found_AuthenticationState { get; }
//    public TypeHelper? Found_StateMapper { get; }
//    public TypeHelper? Found_StateDto { get; }


//    public List<string> CheckForErrors()
//    {
//        var errors = new List<string>();

//        foreach (var hubInterface in HubInterfaces)
//            foreach (var method in hubInterface.Methods)
//                CheckHub(method.ResponseType, errors, method.Name, hubInterface.FullName);

//        foreach (var hubInterface in ApiInterfaces)
//            foreach (var method in hubInterface.Methods)
//                CheckApi(method.ResponseType, errors, method.Name, hubInterface.FullName);

//        foreach (var hubInterface in MinimalApiInterfaces)
//            foreach (var method in hubInterface.Methods)
//                CheckApi(method.ResponseType, errors, method.Name, hubInterface.FullName);

//        return errors;
//    }
//    private void CheckHub(TypeHelper responseType, List<string> errors, string method, string hubInterface)
//    {
//        if (responseType.IsTaskT)
//        {
//            errors.Add(
//                $"Method '{method}' on interface '{hubInterface}' returns Task<T>. " +
//                "Please use IAsyncEnumerable<T> instead. " +
//                "When communicating from server to client, the number of client responses is unknown, " +
//                "therefore response methods must use IAsyncEnumerable<T>.");
//        }
//        else if (!responseType.IsTask && !responseType.IsIAsyncEnumerable)
//        {
//            errors.Add(
//                $"Method '{method}' on interface '{hubInterface}' appears to be synchronous. " +
//                "Please use Task (no response) or IAsyncEnumerable<T> (with responses).");
//        }
//    }
//    private void CheckApi(TypeHelper responseType, List<string> errors, string method, string hubInterface)
//    {
//        if (!responseType.IsTaskT && !responseType.IsTask && !responseType.IsIAsyncEnumerable)
//        {
//            errors.Add(
//                $"Method '{method}' on interface '{hubInterface}' appears to be synchronous. " +
//                "Please use Task (no response), Task<T> or IAsyncEnumerable<T> (with responses).");
//        }
//    }
//}
