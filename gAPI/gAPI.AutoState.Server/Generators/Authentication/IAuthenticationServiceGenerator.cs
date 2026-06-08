//using gAPI.AutoState.Server.Models;
//using gAPI.AutoState.Server.Models.Entities;

//namespace gAPI.AutoState.Server.Generators.Authentication;

//public class IAuthenticationServiceGenerator : _BaseGenerator
//{
//    public IAuthenticationServiceGenerator(
//        Generator context)
//    {
//        Directory = "";
//        Namespace = "gAPI.Generated";

//        Context = context;

//        Name = "IAuthenticationService";
//        FileName = $"{Name}.cs";
//    }

//    public Generator Context { get; }

//    public Entity User => Context.UserEntity;
//    public SharedReference AuthenticationState => Context.AuthenticationState;
//    public SharedReference GapiIAuthenticationService => Context.SharedReferences.IServerAuthenticationService;
//    public SharedReference State => Context.State;

//    public override void GenerateCode()
//    {
//        Reg(User);
//        Reg(State);
//        Reg(AuthenticationState);

//        Code = $@"{GetNamespacesCode()}
//namespace {Namespace};

//public interface {Name} : {GapiIAuthenticationService.FullName}
//{{
//    {State}? ClientState {{ get; }}
//    {AuthenticationState} State {{ get; }}
//    bool Initialized {{ get; }}

//    Task<{AuthenticationState}> AuthenticateUserAsync({User} dbUser, CancellationToken ct);
//    Task LogoutAsync(CancellationToken ct);
//    Task SaveChangesAsync(CancellationToken ct);
//}}";


//    }
//}