using gAPI.CodeGen.Backend.Models.Config;
using gAPI.Core.Helpers;
using UwvLlm.Infrastructure.Data.Entities;

var root = EnvironmentPathHelper.GetRoot(Environment.ProcessPath!, "UwvLlm");
var config = new BackendConfig(
    DbContextType: typeof(ApplicationDbContext),

    Shared_DtosDirectory: EnvironmentPathHelper.GetDirectory(root, @"UwvLlm.Shared.Public\Dtos"),
    Shared_DtosNamespace: "UwvLlm.Shared.Public.Dtos",
    Shared_StateDtosDirectory: EnvironmentPathHelper.GetDirectory(root, @"UwvLlm.Shared.Public\Dtos"),
    Shared_StateDtosNamespace: "UwvLlm.Shared.Public.Dtos",
    Shared_CrudInterfacesDirectory: EnvironmentPathHelper.GetDirectory(root, @"UwvLlm.Shared.Public\CrudInterfaces"),
    Shared_CrudInterfacesNamespace: "UwvLlm.Shared.Public.CrudInterfaces",
    Shared_CrudInterfacesEnd: "Api",

    Core_CrudUseCasesDirectory: EnvironmentPathHelper.GetDirectory(root, @"UwvLlm.Infrastructure.Data\UseCases"),
    Core_CrudUseCasesNamespace: "UwvLlm.Infrastructure.Data.UseCases",
    Core_CrudMappingsDirectory: EnvironmentPathHelper.GetDirectory(root, @"UwvLlm.Infrastructure.Data\Mappings"),
    Core_CrudMappingsNamespace: "UwvLlm.Infrastructure.Data.Mappings",
    Core_CrudServicesDirectory: EnvironmentPathHelper.GetDirectory(root, @"UwvLlm.Infrastructure.Data\CrudServices"),
    Core_CrudServicesNamespace: "UwvLlm.Infrastructure.Data.CrudServices",
    Core_CrudServicesEnd: "Api",

    Extensions_Directory: EnvironmentPathHelper.GetDirectory(root, @"UwvLlm.Infrastructure.Data\Extensions"),
    Extensions_Namespace: "UwvLlm.Infrastructure.Data.Extensions",
    OverwriteServices: true,
    OverwriteServiceInterfaces: true,
    OverwriteMappers: true,
    OverwriteUseCases: false
    );

var generator = new gAPI.CodeGen.Backend.BackendGenerator(config);
generator.Run();