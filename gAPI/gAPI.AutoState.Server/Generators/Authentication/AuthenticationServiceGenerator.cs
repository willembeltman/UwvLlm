//using gAPI.AutoState.Server.Models;

//namespace gAPI.AutoState.Server.Generators.Authentication;

//public class AuthenticationServiceGenerator : _BaseGenerator
//{
//    public AuthenticationServiceGenerator(
//        Generator context)
//    {
//        Directory = "";
//        Namespace = "gAPI.Generated";

//        Context = context;

//        Name = "AuthenticationService";
//        FileName = $"{Name}.cs";
//    }

//    public Generator Context { get; }

//    public SharedReference ApplicationDbContext => Context.DbContext;
//    public IAuthenticationStateFactoryGenerator IAuthenticationStateFactory => Context.IAuthenticationStateFactory;
//    public SharedReference StateMapping => Context.StateMapping;
//    public SharedReference IAuthenticationService => Context.IAuthenticationService;
//    public SharedReference AuthenticationState => Context.AuthenticationState;
//    public SharedReference StateUser => Context.State.User;
//    public SharedReference Token => Context.Token;
//    public SharedReference State => Context.State;
//    public SharedReference AuthenticationHeaders => Context.SharedReferences.AuthenticationHeaders;
//    public SharedReference AuthenticationInitializeResult => Context.SharedReferences.AuthenticationInitializeResult;
//    public SharedReference GapiIAuthenticationService => Context.SharedReferences.GapiIAuthenticationService;
//    public SharedReference User => Context.DbContext.UserEntity;

//    public override void GenerateCode()
//    {
//        Reg("Microsoft.AspNetCore.Http");
//        Reg("Microsoft.EntityFrameworkCore");
//        Reg("Microsoft.Extensions.Hosting");
//        Reg("Microsoft.Extensions.Primitives");
//        Reg("System.Net");
//        Reg("System.Security.Claims");
//        Reg(ApplicationDbContext);
//        Reg(IAuthenticationStateFactory);
//        Reg(StateMapping);
//        Reg(IAuthenticationService);
//        Reg(AuthenticationState);
//        Reg(StateUser);
//        Reg(Token);
//        Reg(State);
//        Reg(AuthenticationHeaders);
//        Reg(AuthenticationInitializeResult);
//        Reg(User);

//        Code = $@"{GetNamespacesCode()}
//namespace {Namespace};

//public class {Name}(
//    {ApplicationDbContext} db,
//    {IAuthenticationStateFactory} factory,
//    IHostEnvironment hostEnvironment)
//    : {IAuthenticationService}
//{{
//    private {AuthenticationHeaders}? Headers;

//    private {State}? ReceivedClientState;
//    private {AuthenticationState}? State;
//    private {AuthenticationInitializeResult}? Result;

//    public bool Initialized {{ get; private set; }}

//    {AuthenticationInitializeResult} {GapiIAuthenticationService.FullName}.Result
//        => Result ?? throw new Exception(""Initialize the AuthenticationService first please"");
//    {State}? {IAuthenticationService}.ClientState
//        => ReceivedClientState;
//    {AuthenticationState} {IAuthenticationService}.State
//        => State ?? throw new Exception(""Initialize the AuthenticationService first please"");
//    string {GapiIAuthenticationService.FullName}.SessionId
//        => Headers?.SessionId ?? throw new Exception(""Initialize the AuthenticationService first please"");
//    string? {GapiIAuthenticationService.FullName}.UserId
//        => State?.DbUser?.Id.ToString();
//    bool {GapiIAuthenticationService.FullName}.UpdateCookie
//        => Headers?.UpdateCookie ?? throw new Exception(""Initialize the AuthenticationService first please"");
//    string? {GapiIAuthenticationService.FullName}.CookieData
//        => Headers?.CookieData;

//    public async Task<{AuthenticationInitializeResult}> InitializeAsync(PathString path, QueryString query, IPAddress? ipAddress, string? cookieData, StringValues sessionData, StringValues stateData, CancellationToken ct)
//    {{
//        if (ipAddress == null)
//        {{
//            Result = new {AuthenticationInitializeResult}()
//            {{
//                Forbidden = true,
//                ForbiddenReason = ""No IP address found.""
//            }};
//            return Result;
//        }}

//        if (hostEnvironment.IsDevelopment() &&
//            (path.ToString().StartsWith(""/scalar"", StringComparison.CurrentCultureIgnoreCase) ||
//            path.ToString().StartsWith(""/openapi"", StringComparison.CurrentCultureIgnoreCase)))
//        {{
//            Result = new {AuthenticationInitializeResult}();
//            return Result;
//        }}

//        var sessionId = sessionData.FirstOrDefault()?.ToString();
//        if (sessionId == null)
//        {{
//            Result = new {AuthenticationInitializeResult}()
//            {{
//                Forbidden = true,
//                ForbiddenReason = ""No session data found.""
//            }};
//            return Result;
//        }}

//        Headers = new AuthenticationHeaders(path, query, ipAddress, cookieData, sessionId);
//        return await Make(Headers, stateData, ct);
//    }}

//    public async Task<{AuthenticationInitializeResult}> ReInitializeAsync(CancellationToken ct)
//    {{
//        if (State == null || Headers == null)
//            throw new Exception(""Initialize the AuthenticationService first please"");

//        var stateData = await GetStateDataAsync(ct);
//        return await Make(Headers, stateData, ct);
//    }}

//    private async Task<{AuthenticationInitializeResult}> Make(AuthenticationHeaders headers, StringValues stateData, CancellationToken ct)
//    {{
//        ReceivedClientState = {State.FullName}.FromStateData(stateData);
//        State = await factory.CreateAuthenticationStateAsync(headers, ReceivedClientState, ct);

//        if (State.DbUser?.LockedOut == true)
//        {{
//            Result = new {AuthenticationInitializeResult}()
//            {{
//                Forbidden = true,
//                ForbiddenReason = ""User is locked out""
//            }};
//            return Result;
//        }}
//        // Additional forbidden checks can be added here

//        Initialized = true;
//        Result = new {AuthenticationInitializeResult}()
//        {{
//            Authenticated = State.DbUser != null
//        }};
//        return Result;
//    }}

//    public async Task<{AuthenticationState}> AuthenticateUserAsync({User} dbUser, CancellationToken ct)
//    {{
//        if (State == null || Headers == null)
//            throw new Exception(""Initialize the AuthenticationService first please"");

//        // Sets cookie data in Headers, and gets new cookie hash
//        var cookieHash = Headers.CreateNewCookie();

//        // Add new token hash to database
//        var dbToken = new {Token}(dbUser, cookieHash);
//        await db.Tokens.AddAsync(dbToken);
//        await db.SaveChangesAsync();

//        // Re-initialize using old header, with new cookie data
//        var initResult = await ReInitializeAsync(ct);
//        if (!initResult.Authenticated)
//            throw new Exception(""Re-initialization after authentication failed"");
//        if (initResult.Forbidden)
//            throw new Exception($""User forbidden: {{initResult.ForbiddenReason}}"");

//        // Return updated state
//        return State;
//    }}
//    public async Task<ClaimsPrincipal> GetClaimsPrincipalAsync(CancellationToken ct)
//    {{
//        if (State == null || Headers == null)
//            throw new Exception(""Initialize the AuthenticationService first please"");

//        if (State.User == null)
//            throw new Exception(""User is not authenticated"");

//        Claim[] claims =
//        [
//            new Claim(ClaimTypes.NameIdentifier, State.User.Id.ToString()),
//            new Claim(ClaimTypes.Name, State.User.Email),
//            new Claim(""UserId"", State.User.Id.ToString())
//        ];

//        var identity = new ClaimsIdentity(claims, authenticationType: ""Cookie"");
//        var user = new ClaimsPrincipal(identity);
//        return user;
//    }}
//    public async Task<StringValues> GetStateDataAsync(CancellationToken ct)
//    {{
//        if (State == null || Headers == null)
//            throw new Exception(""Initialize the AuthenticationService first please"");
//        return State.CreateStateData();
//    }}
//    public async Task LogoutAsync(CancellationToken ct)
//    {{
//        if (State == null || Headers == null)
//            throw new Exception(""Initialize the AuthenticationService first please"");
//        Headers.RemoveCookie();
//        await ReInitializeAsync(ct);
//    }}
//    public async Task SaveChangesAsync(CancellationToken ct)
//        => await db.SaveChangesAsync(ct);
//}}";

//    }
//}