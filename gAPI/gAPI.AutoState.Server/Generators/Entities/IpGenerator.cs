//using gAPI.AutoState.Server.Models;

//namespace gAPI.AutoState.Server.Generators.Entities;

//public class IpGenerator : _BaseGenerator
//{
//    public IpGenerator(
//        Generator context)
//    {
//        Directory = "";
//        Namespace = "gAPI.Generated";

//        Context = context;

//        Name = "Ip";
//        FileName = $"{Name}.cs";
//    }

//    public Generator Context { get; }

//    public SharedReference IsHidden => Context.SharedReferences.IsHidden;
//    public UserIpGenerator UserIp => Context.UserIp;

//    public override void GenerateCode()
//    {
//        Reg("System.ComponentModel.DataAnnotations");
//        Reg(IsHidden);
//        Reg(UserIp);

//        Code = $@"{GetNamespacesCode()}
//namespace {Namespace};

//[{IsHidden}]
//public class Ip
//{{
//    public Ip() {{ }}
//    public Ip(string ipAdress)
//    {{
//        Address = ipAdress;
//    }}

//    [Key]
//    public long Id {{ get; set; }}

//    [StringLength(128)]
//    public string Address {{ get; set; }} = string.Empty;

//    public DateTimeOffset? RegisterLockedOutDate {{ get; set; }}
//    public int RegisterCount {{ get; set; }}
//    public DateTimeOffset? LoginLockedOutDate {{ get; set; }}
//    public int LoginAttempts {{ get; set; }}
//    public DateTimeOffset? ForgetPasswordLockedOutDate {{ get; set; }}
//    public int ForgetPasswordAttempts {{ get; set; }}
//    public int ChangePasswordAttempts {{ get; set; }}
//    public DateTimeOffset? ChangePasswordLockedOutDate {{ get; set; }}

//    public virtual ICollection<{UserIp}>? UserIps {{ get; set; }}
//}}";
//    }
//}