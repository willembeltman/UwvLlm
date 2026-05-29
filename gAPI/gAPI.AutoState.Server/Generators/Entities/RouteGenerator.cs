using gAPI.AutoState.Server.Models;

namespace gAPI.AutoState.Server.Generators.Entities;

public class RouteGenerator : _BaseGenerator
{
    public RouteGenerator(
        Generator context)
    {
        Directory = "";
        Namespace = "gAPI.Generated";

        Context = context;

        Name = "Route";
        FileName = $"{Name}.cs";
    }

    public Generator Context { get; }

    public SharedReference IsHidden => Context.SharedReferences.IsHidden;
    public UserIpSessionTokenRouteGenerator UserIpSessionTokenRoute => Context.UserIpSessionTokenRoute;

    public override void GenerateCode()
    {
        Reg("System.ComponentModel.DataAnnotations");
        Reg("System.ComponentModel.DataAnnotations.Schema");
        Reg(IsHidden);
        Reg(UserIpSessionTokenRoute);

        Code = $@"{GetNamespacesCode()}
namespace {Namespace};

[{IsHidden}]
public class {Name}
{{
    public {Name}() {{ }}
    public {Name}(string route)
    {{
        RouteName = route;
    }}

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id {{ get; set; }}

    public string? RouteName {{ get; set; }} = string.Empty;

    public virtual ICollection<{UserIpSessionTokenRoute}>? UserIpSessionTokenRoutes {{ get; set; }}
}}";
        
    }
}