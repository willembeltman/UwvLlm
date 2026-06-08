//using gAPI.AutoState.Server.Models;

//namespace gAPI.AutoState.Server.Generators.Entities;

//public class UserIpSessionTokenRouteRequestGenerator : _BaseGenerator
//{
//    public UserIpSessionTokenRouteRequestGenerator(
//        Generator context)
//    {
//        Directory = "";
//        Namespace = "gAPI.Generated";

//        Context = context;

//        Name = "UserIpSessionTokenRouteRequest";
//        FileName = $"{Name}.cs";
//    }

//    public Generator Context { get; }

//    public SharedReference IsHidden => Context.SharedReferences.IsHidden;
//    public UserIpSessionTokenRouteGenerator UserIpSessionTokenRoute => Context.UserIpSessionTokenRoute;

//    public override void GenerateCode()
//    {
//        Reg("Microsoft.EntityFrameworkCore");
//        Reg("Microsoft.EntityFrameworkCore.Metadata.Builders");
//        Reg("System.ComponentModel.DataAnnotations");
//        Reg("System.ComponentModel.DataAnnotations.Schema");
//        Reg(IsHidden);
//        Reg(UserIpSessionTokenRoute);

//        Code = $@"{GetNamespacesCode()}
//namespace {Namespace};

//[{IsHidden}]
//public class {Name}
//{{
//    public {Name}() {{ }}
//    public {Name}(
//        {UserIpSessionTokenRoute}? userIpSessionCompanyTokenRoute,
//        DateTimeOffset date)
//    {{
//        UserIpSessionTokenRoute = userIpSessionCompanyTokenRoute;
//        Year = date.Year;
//        Month = Convert.ToByte(date.Month);
//        Day = Convert.ToByte(date.Day);
//        Hour = Convert.ToByte(date.Hour);
//        Count = 1;
//    }}

//    [Key]
//    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//    public long Id {{ get; set; }}

//    public long UserIpSessionTokenRouteId {{ get; set; }}
//    public virtual {UserIpSessionTokenRoute}? UserIpSessionTokenRoute {{ get; set; }}

//    public int Year {{ get; set; }}
//    public byte Month {{ get; set; }}
//    public byte Day {{ get; set; }}
//    public byte Hour {{ get; set; }}

//    public int Count {{ get; set; }}
//}}

//public class {Name}Configuration : IEntityTypeConfiguration<{Name}>
//{{
//    public void Configure(EntityTypeBuilder<{Name}> modelBuilder)
//    {{
//        modelBuilder
//            .HasOne(cb => cb.UserIpSessionTokenRoute)
//            .WithMany(cd => cd.UserIpSessionTokenRouteRequests)
//            .HasForeignKey(cb => cb.UserIpSessionTokenRouteId)
//            .OnDelete(DeleteBehavior.NoAction);
//    }}
//}}";

//    }
//}