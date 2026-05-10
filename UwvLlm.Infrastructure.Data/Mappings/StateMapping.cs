using gAPI.Core.Server.Entities;
using gAPI.Core.Server.Mappings;
using UwvLlm.Infrastructure.Data.Entities;

namespace UwvLlm.Infrastructure.Data.Mappings;

public class StateMapping(
    IStateUserMapping<User, UwvLlm.Shared.Public.Dtos.StateUser> stateUserMapping)
    : IStateMapping<User, UwvLlm.Shared.Public.Dtos.State>
{
    public async Task<UwvLlm.Shared.Public.Dtos.State> ToDtoAsync(
        User? dbUser, 
        UserToken<User>? dbToken, 
        Ip<User> dbIp,
        UwvLlm.Shared.Public.Dtos.State? receivedClientState, 
        CancellationToken ct)
    {
        return new UwvLlm.Shared.Public.Dtos.State
        {
            User = dbUser != null ? await stateUserMapping.ToDtoAsync(dbUser, new UwvLlm.Shared.Public.Dtos.StateUser(), ct) : null
        };
    }
}
