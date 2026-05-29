//using gAPI.AutoState.Server.Models;
//using gAPI.AutoState.Server.Models.Entities;

//namespace gAPI.AutoState.Server.Generators.Authentication;

//public class IServerAuthenticationServiceGenerator : _BaseGenerator
//{
//    public IServerAuthenticationServiceGenerator(
//        Generator context)
//    {
//        Directory = "";
//        Namespace = "gAPI.Generated";

//        Context = context;

//        Name = "IServerAuthenticationService";
//        FileName = $"{Name}.cs";
//    }

//    public Generator Context { get; }

//    public Entity User => Context.DbContext.UserEntity;
//    public SharedReference ServerAuthenticationState => Context.ServerAuthenticationState;
//    public SharedReference GapiIServerAuthenticationService => Context.SharedReferences.GapiIServerAuthenticationService;
//    public SharedReference State => Context.State;

//    public override void GenerateCode()
//    {
//        Reg(User);
//        Reg(State);
//        Reg(ServerAuthenticationState);

//        Code = $@"{GetNamespacesCode()}
//namespace {Namespace};

//public interface {Name} : {GapiIServerAuthenticationService.FullName}
//{{
//    {State}? ClientState {{ get; }}
//    {ServerAuthenticationState} State {{ get; }}
//    bool Initialized {{ get; }}

//    Task<{ServerAuthenticationState}> AuthenticateUserAsync({User} dbUser, CancellationToken ct);
//    Task LogoutAsync(CancellationToken ct);
//    Task SaveChangesAsync(CancellationToken ct);
//}}";

//        
//    }
//}