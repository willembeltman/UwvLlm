using gAPI.AutoSerializer;
using gAPI.AutoState.Client.Helpers;
using gAPI.AutoState.Client.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace gAPI.AutoState.Client;

[Generator]
public class Program : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterSourceOutput(context.CompilationProvider, (spc, compilation) =>
        {
            //#if DEBUG
            //            if (!Debugger.IsAttached)
            //            {
            //                Debugger.Launch(); // Triggert dialoog om te attachen
            //            }
            //#endif

            try
            {
                var allSymbols = compilation.GlobalNamespace.GetAllTypes().ToArray();

                var customSerializers = FindCustomSerializer.GetAllCustomSerializers(allSymbols);
                var customSpanSerializers = FindCustomSerializer.GetAllCustomSpanSerializers(allSymbols);
                var customComparers = FindCustomSerializer.GetAllCustomComparers(allSymbols);
                var customMultipartFormDataContents = FindCustomSerializer.GetAllCustomMultipartFormDataContents(allSymbols);
                var sharedReferences = new SharedReferences(allSymbols);
                var serviceContext = new ServiceContext(allSymbols);
                var serviceModelErrors = serviceContext.CheckForErrors();
                var generator = new Generator(serviceContext, sharedReferences, customSerializers, customSpanSerializers, customComparers, customMultipartFormDataContents);
                generator.Generate(spc);

                if (serviceModelErrors.Count > 0)
                {
                    ShowError(string.Join(", ", serviceModelErrors), spc);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, spc);
                //throw;
            }
        });
    }

    public void ShowError(Exception exception, SourceProductionContext CurrentSpc)
    {
        ShowError(exception.Message, CurrentSpc);
    }

    public void ShowError(string errorMessage, SourceProductionContext CurrentSpc)
    {
        //throw new Exception(errorMessage); // Helps while debugging
        var sourceCode = $"#error gAPI.AutoState.Client: {errorMessage.Replace("\r", "").Replace("\n", " ")}";
        CurrentSpc.AddSource("Gapi_Error.AutoWss.Client.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
    }
}