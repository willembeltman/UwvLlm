//using gAPI.AutoState.Server.Generators.Entities;
//using gAPI.AutoState.Server.Generators.Shared.StateDtos;
//using gAPI.AutoState.Server.Models;
//using gAPI.AutoState.Server.Models.Entities;

//namespace gAPI.AutoState.Server.Generators.Authentication;

//public class ServerAuthenticationStateGenerator : _BaseGenerator
//{
//    public ServerAuthenticationStateGenerator(
//        Generator context)
//    {
//        Directory = "";
//        Namespace = "gAPI.Generated";

//        Context = context;

//        Name = "ServerAuthenticationState";
//        FileName = $"{Name}.cs";
//    }

//    public Generator Context { get; }

//    public SharedReference State => Context.State;
//    public SharedReference StateUser => Context.State.User;
//    public SharedReference Token => Context.Token;
//    public SharedReference User => Context.DbContext.UserEntity;
//    public SharedReference Ip => Context.Ip;

//    public override void GenerateCode()
//    {
//        Reg(State);
//        Reg(StateUser);
//        Reg(Token);
//        Reg(User);
//        Reg(Ip);

//        Code = $@"{GetNamespacesCode()}
//namespace {Namespace};

//public class {Name} : {State}
//{{
//    public {Name}({StateUser}? user, {Token}? dbToken, {User}? dbUser, {Ip} dbIp)
//    {{
//        User = user;
//        DbToken = dbToken;
//        DbUser = dbUser;
//        DbIp = dbIp;
//    }}

//    public {Token}? DbToken {{ get; }}
//    public {User}? DbUser {{ get; }}
//    public {Ip} DbIp {{ get; }}
//}}";
//        
//    }
//}