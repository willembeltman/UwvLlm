//using gAPI.AutoState.Server.Models;

//namespace gAPI.AutoState.Server.Generators.Authentication;

//public class IServerAuthenticationStateFactoryGenerator : _BaseGenerator
//{
//    public IServerAuthenticationStateFactoryGenerator(
//        Generator context)
//    {
//        Directory = "";
//        Namespace = "gAPI.Generated";

//        Context = context;

//        Name = "IServerAuthenticationStateFactory";
//        FileName = $"{Name}.cs";
//    }

//    public Generator Context { get; }

//    public SharedReference ServerAuthenticationState => Context.ServerAuthenticationState;
//    public SharedReference AuthenticationHeaders => Context.SharedReferences.AuthenticationHeaders;
//    public SharedReference State => Context.State;

//    public override void GenerateCode()
//    {
//        Reg(ServerAuthenticationState);
//        Reg(AuthenticationHeaders);
//        Reg(State);

//        Code = $@"{GetNamespacesCode()}
//namespace {Namespace};

//public interface {Name}
//{{
//    Task<{ServerAuthenticationState}> CreateAuthenticationStateAsync({AuthenticationHeaders} headers, {State}? stateData, CancellationToken ct);
//}}";
//        
//    }
//}