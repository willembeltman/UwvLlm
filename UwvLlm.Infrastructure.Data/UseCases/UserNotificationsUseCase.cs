using gAPI.Core.Server.Interfaces;
using Microsoft.EntityFrameworkCore;
using UwvLlm.Infrastructure.Data.Entities;

namespace UwvLlm.Infrastructure.Data.UseCases;

public class UserNotificationsUseCase(
    ApplicationDbContext db,
    IAuthenticationService<User, UwvLlm.Shared.Public.Dtos.State> authenticationService)
    : gAPI.Core.Interfaces.IUseCase<UserNotification, UwvLlm.Shared.Public.Dtos.UserNotification, long>
{
    private Guid? CurrentUserId => authenticationService.State.User?.Id;

    public async Task<bool> IsAllowedAsync(CancellationToken ct) => CurrentUserId != null;
    public async Task<bool> CanListAsync(CancellationToken ct) => CurrentUserId != null;
    public async Task<bool> CanCreateAsync(CancellationToken ct) => authenticationService.State.User != null;
    public async Task<bool> CanCreateAsync(UwvLlm.Shared.Public.Dtos.UserNotification dto, CancellationToken ct) => CurrentUserId != null;
    public async Task<bool> CanReadAsync(UwvLlm.Shared.Public.Dtos.UserNotification dto, CancellationToken ct) => dto.UserId == CurrentUserId;
    public async Task<bool> CanUpdateAsync(UwvLlm.Shared.Public.Dtos.UserNotification dto, CancellationToken ct) => dto.UserId == CurrentUserId;
    public async Task<bool> CanDeleteAsync(UwvLlm.Shared.Public.Dtos.UserNotification dto, CancellationToken ct) => dto.UserId == CurrentUserId;

    public async Task<UserNotification?> FindByMatchAsync(UwvLlm.Shared.Public.Dtos.UserNotification dto, CancellationToken ct) 
        => null; // If you implement this, also use includes
    public async Task<UserNotification?> FindByIdAsync(long id, CancellationToken ct) 
        => await ListAll()
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id, ct);
    public IQueryable<UserNotification> ListAll()
    {
        var currentUserId = CurrentUserId;
        if (currentUserId == null)
        {
            return db.UserNotifications.Where(a => false);
        }

        return db.UserNotifications.Where(a => a.UserId == currentUserId);
    }

    public async Task<bool> AddAsync(UserNotification entityToAdd, CancellationToken ct) 
    {
        var currentUserId = CurrentUserId;
        if (currentUserId == null) return false;

        entityToAdd.UserId = currentUserId.Value;
        entityToAdd.User = null;

        await db.UserNotifications.AddAsync(entityToAdd, ct);
        await db.SaveChangesAsync(ct);
        return true;
    }
    public async Task<bool> UpdateAsync(UserNotification updatedEntity, UwvLlm.Shared.Public.Dtos.UserNotification dto, CancellationToken ct)
    {
        var userId = db.Entry(updatedEntity).Property(a => a.UserId).OriginalValue;
        if (CurrentUserId != userId) return false;

        updatedEntity.UserId = userId;
        updatedEntity.User = null;

        await db.SaveChangesAsync(ct);
        return true;
    }
    public async Task<bool> RemoveAsync(UserNotification entity, CancellationToken ct)
    {
        if (CurrentUserId != entity.UserId) return false;

        db.UserNotifications.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
