using gAPI.Core.Server;
using Microsoft.EntityFrameworkCore;
using UwvLlm.Infrastructure.Data.Entities;

namespace UwvLlm.Infrastructure.Data.UseCases;

public class MailMessagesUseCase(
    ApplicationDbContext db,
    IAuthenticationService<User, UwvLlm.Shared.Public.Dtos.State> authenticationService)
    : gAPI.Core.Interfaces.IUseCase<MailMessage, UwvLlm.Shared.Public.Dtos.MailMessage, Guid>
{
    private Guid? CurrentUserId => authenticationService.State.User?.Id;

    public async Task<bool> IsAllowedAsync(CancellationToken ct) => CurrentUserId != null;
    public async Task<bool> CanListAsync(CancellationToken ct) => CurrentUserId != null;
    public async Task<bool> CanCreateAsync(CancellationToken ct) => authenticationService.State.User != null;
    public async Task<bool> CanCreateAsync(UwvLlm.Shared.Public.Dtos.MailMessage dto, CancellationToken ct) => true; // All emails will be created by the current user, so no need to check the dto
    public async Task<bool> CanReadAsync(UwvLlm.Shared.Public.Dtos.MailMessage dto, CancellationToken ct) 
        => IsCurrentUserParticipant(dto);
    public async Task<bool> CanUpdateAsync(UwvLlm.Shared.Public.Dtos.MailMessage dto, CancellationToken ct) 
        => IsCurrentUserParticipant(dto);
    public async Task<bool> CanDeleteAsync(UwvLlm.Shared.Public.Dtos.MailMessage dto, CancellationToken ct) 
        => IsCurrentUserParticipant(dto);

    public async Task<MailMessage?> FindByMatchAsync(UwvLlm.Shared.Public.Dtos.MailMessage dto, CancellationToken ct) 
        => null; // If you implement this, also use includes
    public async Task<MailMessage?> FindByIdAsync(Guid id, CancellationToken ct) 
        => await ListAll()
            .Include(a => a.FromUser)
            .Include(a => a.ToUser)
            .FirstOrDefaultAsync(a => a.Id == id, ct);
    public IQueryable<MailMessage> ListAll()
    {
        var currentUserId = CurrentUserId;
        if (currentUserId == null)
        {
            return db.MailMessages.Where(a => false);
        }

        return db.MailMessages.Where(a => a.FromUserId == currentUserId || a.ToUserId == currentUserId);
    }

    public async Task<bool> AddAsync(MailMessage entityToAdd, CancellationToken ct) 
    {
        var currentUserId = CurrentUserId;
        if (currentUserId == null) return false;

        entityToAdd.FromUserId = currentUserId.Value;
        entityToAdd.FromUser = null!;

        await db.MailMessages.AddAsync(entityToAdd, ct);
        await db.SaveChangesAsync(ct);
        return true;
    }
    public async Task<bool> UpdateAsync(MailMessage updatedEntity, UwvLlm.Shared.Public.Dtos.MailMessage dto, CancellationToken ct)
    {
        var entry = db.Entry(updatedEntity);
        var fromUserId = entry.Property(a => a.FromUserId).OriginalValue;
        var toUserId = entry.Property(a => a.ToUserId).OriginalValue;

        if (CurrentUserId != fromUserId && CurrentUserId != toUserId) return false;

        updatedEntity.FromUserId = fromUserId;
        updatedEntity.ToUserId = toUserId;
        updatedEntity.FromUser = null!;
        updatedEntity.ToUser = null!;

        await db.SaveChangesAsync(ct);
        return true;
    }
    public async Task<bool> RemoveAsync(MailMessage entity, CancellationToken ct)
    {
        if (CurrentUserId != entity.FromUserId && CurrentUserId != entity.ToUserId) return false;

        db.MailMessages.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private bool IsCurrentUserParticipant(UwvLlm.Shared.Public.Dtos.MailMessage dto)
        => CurrentUserId != null && (dto.FromUserId == CurrentUserId || dto.ToUserId == CurrentUserId);
}
