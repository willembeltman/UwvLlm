//using gAPI.AutoState.Server.Models;
//using gAPI.AutoState.Server.Models.Entities;

//namespace gAPI.AutoState.Server.Generators.Entities;

//public class TokenGenerator : _BaseGenerator
//{
//    public TokenGenerator(
//        Generator context)
//    {
//        Directory = "";
//        Namespace = "gAPI.Generated";

//        Context = context;

//        Name = "Token";
//        FileName = $"{Name}.cs";
//    }

//    public Generator Context { get; }

//    public UserIpSessionTokenGenerator UserIpSessionToken => Context.UserIpSessionToken;
//    public SharedReference IsHidden => Context.SharedReferences.IsHidden;
//    public Entity User => Context.UserEntity;

//    public override void GenerateCode()
//    {
//        Reg("System.ComponentModel.DataAnnotations");
//        Reg("Microsoft.EntityFrameworkCore");
//        Reg("Microsoft.EntityFrameworkCore.Metadata.Builders");
//        Reg(IsHidden);
//        Reg(User);
//        Reg(UserIpSessionToken);

//        Code = $@"{GetNamespacesCode()}
//namespace {Namespace};

//[IsHidden]
//public class {Name}
//{{
//    public {Name}() {{ }}
//    public {Name}(User user, string tokenHash)
//    {{
//        User = user;
//        TokenHash = tokenHash;
//    }}

//    [Key]
//    public long Id {{ get; set; }}

//    public {User.KeyProperty.Type} UserId {{ get; set; }}
//    public virtual {User}? User {{ get; set; }}

//    [StringLength(280)]
//    public string TokenHash {{ get; set; }} = string.Empty;
//    public DateTime Date {{ get; set; }} = DateTime.Now;

//    public virtual ICollection<{UserIpSessionToken}>? UserIpSessionTokens {{ get; set; }}

//}}

//public class {Name}Configuration : IEntityTypeConfiguration<{Name}>
//{{
//    public void Configure(EntityTypeBuilder<{Name}> modelBuilder)
//    {{
//        modelBuilder
//            .HasOne(cb => cb.User)
//            .WithMany(u => u.Tokens)
//            .HasForeignKey(cb => cb.UserId)
//            .OnDelete(DeleteBehavior.NoAction);
//    }}
//}}";

//    }
//}