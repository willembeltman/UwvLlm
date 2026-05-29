using gAPI.AutoSerializer;
using gAPI.AutoState.Generators;
using gAPI.AutoState.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.IO;
using System.Text;

namespace gAPI.AutoState;

public class Generator
{
    public Generator(
        ServiceContext serviceContext,
        SharedReferences sharedReferences,
        CustomObject[] customSerializers,
        CustomObject[] customSpanSerializers,
        CustomObjectMethod[] customComparers,
        CustomObjectMethod[] customMultipartFormDataContentSerializers)
    {
        ServiceContext = serviceContext;
        SharedReferences = sharedReferences;
        CustomSerializers = customSerializers;
        CustomSpanSerializers = customSpanSerializers;
        CustomComparers = customComparers;
        CustomMultipartFormDataContentSerializers = customMultipartFormDataContentSerializers;

    }
    public ServiceContext ServiceContext { get; }
    public SharedReferences SharedReferences { get; }
    public CustomObject[] CustomSerializers { get; }
    public CustomObject[] CustomSpanSerializers { get; }
    public CustomObjectMethod[] CustomComparers { get; }
    public CustomObjectMethod[] CustomMultipartFormDataContentSerializers { get; }

    public void Generate(SourceProductionContext spc)
    {
    }

    private static void GenerateItem(SourceProductionContext spc, _BaseGenerator generator)
    {
        generator.GenerateCode();

        if (!string.IsNullOrEmpty(generator.Code))
        {
            var signalRHubFullName = Path.Combine(generator.Directory, generator.FileName);
            spc.AddSource(signalRHubFullName, SourceText.From(generator.Code, Encoding.UTF8));
        }
    }
}