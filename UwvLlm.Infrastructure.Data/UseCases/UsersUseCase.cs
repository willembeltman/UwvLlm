using gAPI.Core.Server;
using Microsoft.EntityFrameworkCore;
using UwvLlm.Infrastructure.Data.Entities;

namespace UwvLlm.Infrastructure.Data.UseCases;

public class UsersUseCase(
    ApplicationDbContext db,
    IAuthenticationService<User, UwvLlm.Shared.Public.Dtos.State> authenticationService)
    : gAPI.Core.Interfaces.IUseCase<User, UwvLlm.Shared.Public.Dtos.User, Guid>
{
    private Guid? CurrentUserId => authenticationService.State.User?.Id;

    public async Task<bool> IsAllowedAsync(CancellationToken ct) => CurrentUserId != null;
    public async Task<bool> CanListAsync(CancellationToken ct) => CurrentUserId != null;
    public async Task<bool> CanCreateAsync(CancellationToken ct) => CurrentUserId != null;
    public async Task<bool> CanCreateAsync(UwvLlm.Shared.Public.Dtos.User dto, CancellationToken ct) => CurrentUserId != null;
    public async Task<bool> CanReadAsync(UwvLlm.Shared.Public.Dtos.User dto, CancellationToken ct) => CurrentUserId != null;
    public async Task<bool> CanUpdateAsync(UwvLlm.Shared.Public.Dtos.User dto, CancellationToken ct) => dto.Id == CurrentUserId;
    public async Task<bool> CanDeleteAsync(UwvLlm.Shared.Public.Dtos.User dto, CancellationToken ct) => dto.Id == CurrentUserId;

    public async Task<User?> FindByMatchAsync(UwvLlm.Shared.Public.Dtos.User dto, CancellationToken ct) 
        => await db.Users // Add your filter query
            .FirstOrDefaultAsync(a => 
                a.UserName == dto.UserName, ct);
    public async Task<User?> FindByIdAsync(Guid id, CancellationToken ct) 
        => await db.Users // Add your filter query
            .FirstOrDefaultAsync(a => a.Id == id, ct);
    public IQueryable<User> ListAll()
        => db.Users; // Add your filter query, no need for includes here

    public async Task<bool> AddAsync(User entityToAdd, CancellationToken ct) 
    {
        await db.Users.AddAsync(entityToAdd, ct);
        await db.SaveChangesAsync(ct);
        return true;
    }
    public async Task<bool> UpdateAsync(User updatedEntity, UwvLlm.Shared.Public.Dtos.User dto, CancellationToken ct)
    {
        if (updatedEntity.Id != CurrentUserId) return false;

        await db.SaveChangesAsync(ct);
        return true;
    }
    public async Task<bool> RemoveAsync(User entity, CancellationToken ct)
    {
        if (entity.Id != CurrentUserId) return false;

        db.Users.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
