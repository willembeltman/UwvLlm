using gAPI.AutoState.Server.Models;

namespace gAPI.AutoState.Server.Generators.Entities;

public class UserIpGenerator : _BaseGenerator
{
    public UserIpGenerator(
        Generator context)
    {
        Directory = "";
        Namespace = "gAPI.Generated";

        Context = context;

        Name = "UserIp";
        FileName = $"{Name}.cs";
    }

    public Generator Context { get; }

    public Entity User => Context.UserEntity;
    public SharedReference IsHidden => Context.SharedReferences.IsHidden;
    public IpGenerator Ip => Context.Ip;
    public UserIpSessionGenerator UserIpSession => Context.UserIpSession;

    public override void GenerateCode()
    {
        Reg("Microsoft.EntityFrameworkCore");
        Reg("Microsoft.EntityFrameworkCore.Metadata.Builders");
        Reg("System.ComponentModel.DataAnnotations");
        Reg("System.ComponentModel.DataAnnotations.Schema");
        Reg(IsHidden);
        Reg(User);
        Reg(Ip);
        Reg(UserIpSession);

        Code = $@"{GetNamespacesCode()}
namespace {Namespace};

[{IsHidden}]
public class {Name}
{{
    public {Name}() {{ }}
    public {Name}(
        User? user,
        Ip ip)
    {{
        User = user;
        Ip = ip;
    }}

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id {{ get; set; }}

    public {User.KeyProperty.Type}? UserId {{ get; set; }}
    public virtual {User}? User {{ get; set; }}

    public long IpId {{ get; set; }}
    public virtual {Ip}? Ip {{ get; set; }}

    public virtual ICollection<{UserIpSession}>? UserIpSessions {{ get; set; }}

}}

public class {Name}AddressConfiguration : IEntityTypeConfiguration<{Name}>
{{
    public void Configure(EntityTypeBuilder<{Name}> modelBuilder)
    {{
        modelBuilder
            .HasOne(cb => cb.User)
            .WithMany(cd => cd.UserIps)
            .HasForeignKey(cb => cb.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder
            .HasOne(cb => cb.Ip)
            .WithMany(cd => cd.UserIps)
            .HasForeignKey(cb => cb.IpId)
            .OnDelete(DeleteBehavior.NoAction);
    }}
}}";

    }
}