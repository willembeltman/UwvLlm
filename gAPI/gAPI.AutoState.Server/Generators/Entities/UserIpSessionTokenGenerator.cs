//using gAPI.AutoState.Server.Models;

//namespace gAPI.AutoState.Server.Generators.Entities;

//public class UserIpSessionTokenGenerator : _BaseGenerator
//{
//    public UserIpSessionTokenGenerator(
//        Generator context)
//    {
//        Directory = "";
//        Namespace = "gAPI.Generated";

//        Context = context;

//        Name = "UserIpSessionToken";
//        FileName = $"{Name}.cs";
//    }

//    public Generator Context { get; }

//    public SharedReference IsHidden => Context.SharedReferences.IsHidden;
//    public TokenGenerator Token => Context.Token;
//    public UserIpSessionGenerator UserIpSession => Context.UserIpSession;
//    public UserIpSessionTokenRouteGenerator UserIpSessionTokenRoute => Context.UserIpSessionTokenRoute;

//    public override void GenerateCode()
//    {
//        Reg("Microsoft.EntityFrameworkCore");
//        Reg("Microsoft.EntityFrameworkCore.Metadata.Builders");
//        Reg("System.ComponentModel.DataAnnotations");
//        Reg("System.ComponentModel.DataAnnotations.Schema");
//        Reg(IsHidden);
//        Reg(Token);
//        Reg(UserIpSession);
//        Reg(UserIpSessionTokenRoute);

//        Code = $@"{GetNamespacesCode()}
//namespace {Namespace};

//[{IsHidden}]
//public class {Name}
//{{
//    public {Name}() {{ }}
//    public {Name}(
//        {UserIpSession} userIpSessionCompany,
//        {Token}? token)
//    {{
//        {UserIpSession} = userIpSessionCompany;
//        {Token} = token;
//    }}

//    [Key]
//    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//    public long Id {{ get; set; }}

//    public long? TokenId {{ get; set; }}
//    public virtual {Token}? Token {{ get; set; }}

//    public long UserIpSessionId {{ get; set; }}
//    public virtual {UserIpSession}? UserIpSession {{ get; set; }}

//    public virtual ICollection<{UserIpSessionTokenRoute}>? UserIpSessionTokenRoutes {{ get; set; }}

//}}

//public class {Name}Configuration : IEntityTypeConfiguration<{Name}>
//{{
//    public void Configure(EntityTypeBuilder<{Name}> modelBuilder)
//    {{
//        modelBuilder
//            .HasOne(cb => cb.UserIpSession)
//            .WithMany(cd => cd.UserIpSessionTokens)
//            .HasForeignKey(cb => cb.UserIpSessionId)
//            .OnDelete(DeleteBehavior.NoAction);

//        modelBuilder
//            .HasOne(cb => cb.Token)
//            .WithMany(cd => cd.UserIpSessionTokens)
//            .HasForeignKey(cb => cb.TokenId)
//            .OnDelete(DeleteBehavior.NoAction);
//    }}
//}}";

//    }
//}