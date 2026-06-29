using gAPI.Core.Server.Entities;

namespace UwvLlm.Infrastructure.Data.Entities;

public class User : AuthUser
{
    public virtual ICollection<UserNotification>? Notifications { get; set; }
    public virtual ICollection<MailMessage>? FromMailMessages { get; set; }
    public virtual ICollection<MailMessage>? ToMailMessages { get; set; }
}
