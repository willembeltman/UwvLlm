//using gAPI.AutoState.Server.Models;

//namespace gAPI.AutoState.Server.Generators.Entities;

//public class SessionGenerator : _BaseGenerator
//{
//    public SessionGenerator(
//        Generator context)
//    {
//        Directory = "";
//        Namespace = "gAPI.Generated";

//        Context = context;

//        Name = "Session";
//        FileName = $"{Name}.cs";
//    }

//    public Generator Context { get; }

//    public SharedReference IsHidden => Context.SharedReferences.IsHidden;
//    public UserIpSessionGenerator UserIpSession => Context.UserIpSession;

//    public override void GenerateCode()
//    {
//        Reg("System.ComponentModel.DataAnnotations");
//        Reg(IsHidden);
//        Reg(UserIpSession);

//        Code = $@"{GetNamespacesCode()}
//namespace {Namespace};

//[{IsHidden}]
//public class {Name}
//{{
//    public {Name}() {{ }}
//    public {Name}(string sessionId)
//    {{
//        SessionId = sessionId;
//    }}

//    [Key]
//    public long Id {{ get; set; }}

//    [StringLength(256)]
//    public string SessionId {{ get; set; }} = string.Empty;

//    public virtual ICollection<{UserIpSession}>? UserIpSessions {{ get; set; }}
//}}";

//    }
//}