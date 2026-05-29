using gAPI.AutoState.Server.Models;

namespace gAPI.AutoState.Server.Generators.Entities;

public class UserIpSessionGenerator : _BaseGenerator
{
    public UserIpSessionGenerator(
        Generator context)
    {
        Directory = "";
        Namespace = "gAPI.Generated";

        Context = context;

        Name = "UserIpSession";
        FileName = $"{Name}.cs";
    }

    public Generator Context { get; }

    public SharedReference IsHidden => Context.SharedReferences.IsHidden;
    public UserIpGenerator UserIp => Context.UserIp;
    public SessionGenerator Session => Context.Session;
    public UserIpSessionTokenGenerator UserIpSessionToken => Context.UserIpSessionToken;

    public override void GenerateCode()
    {
        Reg("Microsoft.EntityFrameworkCore");
        Reg("Microsoft.EntityFrameworkCore.Metadata.Builders");
        Reg("System.ComponentModel.DataAnnotations");
        Reg("System.ComponentModel.DataAnnotations.Schema");
        Reg(IsHidden);
        Reg(UserIp);
        Reg(Session);
        Reg(UserIpSessionToken);

        Code = $@"{GetNamespacesCode()}
namespace {Namespace};

[{IsHidden}]
public class {Name}
{{
    public {Name}() {{ }}
    public {Name}(
        UserIp userIp,
        Session session)
    {{
        UserIp = userIp;
        Session = session;
    }}

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id {{ get; set; }}

    public long UserIpId {{ get; set; }}
    public virtual {UserIp}? UserIp {{ get; set; }}

    public long SessionId {{ get; set; }}
    public virtual {Session}? Session {{ get; set; }}

    public virtual ICollection<{UserIpSessionToken}>? UserIpSessionTokens {{ get; set; }}

}}

public class {Name}Configuration : IEntityTypeConfiguration<{Name}>
{{
    public void Configure(EntityTypeBuilder<{Name}> modelBuilder)
    {{
        modelBuilder
            .HasOne(cb => cb.UserIp)
            .WithMany(cd => cd.UserIpSessions)
            .HasForeignKey(cb => cb.UserIpId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder
            .HasOne(cb => cb.Session)
            .WithMany(cd => cd.UserIpSessions)
            .HasForeignKey(cb => cb.SessionId)
            .OnDelete(DeleteBehavior.NoAction);
    }}
}}";

    }
}