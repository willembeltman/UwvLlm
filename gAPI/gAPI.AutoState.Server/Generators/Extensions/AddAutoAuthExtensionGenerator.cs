using gAPI.AutoState.Server.Generators;

namespace gAPI.AutoState.Server.Generators.Extensions;

public class AddAutoAuthExtensionGenerator : _BaseGenerator
{
    public AddAutoAuthExtensionGenerator(Generator generator)
    {
        Context = generator;

        Directory = "";
        Namespace = "gAPI.Generated";

        Name = "AddAutoAuthExtension";
        FileName = $"{Name}.g.cs";
    }

    public Generator Context { get; }

    public override void GenerateCode()
    {
        Code = "";
        
    }
}