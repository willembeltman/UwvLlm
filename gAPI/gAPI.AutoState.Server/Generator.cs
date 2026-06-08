using gAPI.AutoSerializer;
using gAPI.AutoState.Server.Generators;
using gAPI.AutoState.Server.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.IO;
using System.Text;
using gAPI.AutoState.Server.Models.Interfaces;

namespace gAPI.AutoState.Server;

public class Generator
{
    public Generator(
        ServiceContext serviceContext,
        SharedReferences sharedReferences,
        CustomObject[] customSerializers,
        CustomObject[] customSpanSerializers,
        CustomObjectMethod[] customComparers)
    {
        ServiceContext = serviceContext;
        SharedReferences = sharedReferences;
        CustomSerializers = customSerializers;
        CustomSpanSerializers = customSpanSerializers;
        CustomComparers = customComparers;

        //Ip = new IpGenerator(this);
        //Route = new RouteGenerator(this);
        //Session = new SessionGenerator(this);
        //Token = new TokenGenerator(this);
        //UserIp = new UserIpGenerator(this);
        //UserIpSession = new UserIpSessionGenerator(this);
        //UserIpSessionToken = new UserIpSessionTokenGenerator(this);
        //UserIpSessionTokenRoute = new UserIpSessionTokenRouteGenerator(this);
        //UserIpSessionTokenRouteRequest = new UserIpSessionTokenRouteRequestGenerator(this);

        //AddAutoAuthExtension = new AddAutoAuthExtensionGenerator(this);
    }

    public ServiceContext ServiceContext { get; }
    public SharedReferences SharedReferences { get; }
    public CustomObject[] CustomSerializers { get; }
    public CustomObject[] CustomSpanSerializers { get; }
    public CustomObjectMethod[] CustomComparers { get; }

    //public IpGenerator Ip { get; }
    //public RouteGenerator Route { get; }
    //public SessionGenerator Session { get; }
    //public TokenGenerator Token { get; }
    //public UserIpGenerator UserIp { get; }
    //public UserIpSessionGenerator UserIpSession { get; }
    //public UserIpSessionTokenGenerator UserIpSessionToken { get; }
    //public UserIpSessionTokenRouteGenerator UserIpSessionTokenRoute { get; }
    //public UserIpSessionTokenRouteRequestGenerator UserIpSessionTokenRouteRequest { get; }

    //public AddAutoAuthExtensionGenerator AddAutoAuthExtension { get; }
    //public IAuthenticationServiceGenerator IAuthenticationService { get; }
    //public SharedReference DbContext { get; internal set; }
    //public IAuthenticationStateFactoryGenerator IAuthenticationStateFactory { get; internal set; }

    public void Generate(SourceProductionContext spc)
    {
        //GenerateItem(spc, AddAutoAuthExtension);
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