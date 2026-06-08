//using gAPI.AutoState.Server.Models;

//namespace gAPI.AutoState.Server.Generators.Authentication;

//public class IAuthenticationStateFactoryGenerator : _BaseGenerator
//{
//    public IAuthenticationStateFactoryGenerator(
//        Generator context)
//    {
//        Directory = "";
//        Namespace = "gAPI.Generated";

//        Context = context;

//        Name = "IAuthenticationStateFactory";
//        FileName = $"{Name}.cs";
//    }

//    public Generator Context { get; }

//    public SharedReference AuthenticationState => Context.AuthenticationState;
//    public SharedReference AuthenticationHeaders => Context.SharedReferences.AuthenticationHeaders;
//    public SharedReference State => Context.State;

//    public override void GenerateCode()
//    {
//        Reg(AuthenticationState);
//        Reg(AuthenticationHeaders);
//        Reg(State);

//        Code = $@"{GetNamespacesCode()}
//namespace {Namespace};

//public interface {Name}
//{{
//    Task<{AuthenticationState}> CreateAuthenticationStateAsync({AuthenticationHeaders} headers, {State}? stateData, CancellationToken ct);
//}}";

//    }
//}