using Messenger.Domain.Entities.Abstraction;

namespace Messenger.Domain.Entities;

public class Session : IBaseEntity
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }

    public User User { get; set; }

    public string AccessToken { get; set; }

    public Guid RefreshToken { get; set; } = Guid.NewGuid();

    public string Ip { get; set; }
    
    public string UserAgent { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime CreateAt { get; set; } = DateTime.UtcNow;

    public Session(Guid userId, string ip, string userAgent, DateTime expiresAt, string accessToken)
    {
        AccessToken = accessToken;
        UserId = userId;
        Ip = ip;
        UserAgent = userAgent;
        ExpiresAt = expiresAt;
    }
    
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}