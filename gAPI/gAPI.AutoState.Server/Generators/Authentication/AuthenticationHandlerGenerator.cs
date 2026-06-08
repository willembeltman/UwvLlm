//namespace gAPI.AutoState.Server.Generators.Authentication;

//public class AuthenticationHandlerGenerator : _BaseGenerator
//{
//    public AuthenticationHandlerGenerator(
//        Generator context)
//    {
//        Directory = "";
//        Namespace = "gAPI.Generated";

//        Context = context;

//        Name = "AuthenticationHandler";
//        FileName = $"{Name}.cs";
//    }

//    public Generator Context { get; }

//    public IAuthenticationServiceGenerator IAuthenticationService => Context.IAuthenticationService;

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
//    private readonly {IAuthenticationService} Authentication;

//    public AuthenticationHandler(
//        IOptionsMonitor<AuthenticationSchemeOptions> options,
//        ILoggerFactory logger,
//        UrlEncoder encoder,
//        {IAuthenticationService} auth)
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

//    }
//}