//namespace gAPI.AutoState.Server.Generators.Authentication;

//public class ServerAuthenticationHandlerGenerator : _BaseGenerator
//{
//    public ServerAuthenticationHandlerGenerator(
//        Generator context)
//    {
//        Directory = "";
//        Namespace = "gAPI.Generated";

//        Context = context;

//        Name = "ServerAuthenticationHandler";
//        FileName = $"{Name}.cs";
//    }

//    public Generator Context { get; }

//    public IServerAuthenticationServiceGenerator IServerAuthenticationService => Context.IServerAuthenticationService;

//    public override void GenerateCode()
//    {
//        Reg("Microsoft.AspNetCore.Authentication");
//        Reg("Microsoft.Extensions.Logging");
//        Reg("Microsoft.Extensions.Options");
//        Reg("System.Text.Encodings.Web");

//        Code = $@"{GetNamespacesCode()}
//namespace {Namespace};

//public class {Name}
//    : AuthenticationHandler<AuthenticationSchemeOptions>
//{{
//    private readonly {IServerAuthenticationService} Authentication;

//    public ServerAuthenticationHandler(
//        IOptionsMonitor<AuthenticationSchemeOptions> options,
//        ILoggerFactory logger,
//        UrlEncoder encoder,
//        {IServerAuthenticationService} auth)
//        : base(options, logger, encoder)
//    {{
//        Authentication = auth;
//    }}

//    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
//    {{
//        if (Authentication.Initialized == false)
//            return AuthenticateResult.NoResult();

//        if (Authentication.Result.Forbidden)
//            return AuthenticateResult.Fail(
//                Authentication.Result.ForbiddenReason ?? ""Forbidden"");

//        if (!Authentication.Result.Authenticated)
//            return AuthenticateResult.NoResult();

//        var principal = await Authentication.GetClaimsPrincipalAsync(Context.RequestAborted);
//        return AuthenticateResult.Success(
//            new AuthenticationTicket(principal, ""gAPI""));
//    }}
//}}";
//        
//    }
//}