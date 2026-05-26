using gAPI.Core.Attributes;
using gAPI.Core.Dtos;
using UwvLlm.Shared.Public.Dtos;

namespace UwvLlm.Shared.Public.CrudInterfaces;

[GenerateApi]
public interface IMailMessagesCrudService
{
    [IsCreate]
    Task<BaseResponseT<MailMessage>> Create(MailMessage mailmessage, CancellationToken ct);

    [IsRead]
    Task<BaseResponseT<MailMessage>> Read(Guid mailmessageId, CancellationToken ct);

    [IsUpdate]
    Task<BaseResponseT<MailMessage>> Update(MailMessage mailmessage, CancellationToken ct);

    [IsDelete(typeof(MailMessage))]
    Task<BaseResponseT<bool>> Delete(Guid mailmessageId, CancellationToken ct);

    [IsList]
    Task<BaseListResponseT<MailMessage>> List(int? skip, int? take, string[]? orderby, CancellationToken ct);

    [IsListBy(nameof(MailMessage.FromUserId), typeof(User))]
    Task<BaseListResponseT<MailMessage>> ListByFromUserId(Guid FromUserId, int? skip, int? take, string[]? orderby, CancellationToken ct);

    [IsListNotBy(nameof(MailMessage.FromUserId), typeof(User))]
    Task<BaseListResponseT<MailMessage>> ListNotByFromUserId(Guid FromUserId, int? skip, int? take, string[]? orderby, CancellationToken ct);

    [IsListBy(nameof(MailMessage.ToUserId), typeof(User))]
    Task<BaseListResponseT<MailMessage>> ListByToUserId(Guid ToUserId, int? skip, int? take, string[]? orderby, CancellationToken ct);

    [IsListNotBy(nameof(MailMessage.ToUserId), typeof(User))]
    Task<BaseListResponseT<MailMessage>> ListNotByToUserId(Guid ToUserId, int? skip, int? take, string[]? orderby, CancellationToken ct);
}