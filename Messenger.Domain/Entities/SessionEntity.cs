using FluentValidation;
using Messenger.Domain.Entities.Abstraction;
using Messenger.Domain.Entities.Validation;

namespace Messenger.Domain.Entities;

public class SessionEntity : IBaseEntity
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }

    public UserEntity User { get; set; }

    public string AccessToken { get; set; }

    public Guid RefreshToken { get; set; } = Guid.NewGuid();

    public string Ip { get; set; }
    
    public string UserAgent { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime CreateAt { get; set; } = DateTime.UtcNow;

    public SessionEntity(Guid userId, string ip, string userAgent, DateTime expiresAt, string accessToken)
    {
        AccessToken = accessToken;
        UserId = userId;
        Ip = ip;
        UserAgent = userAgent;
        ExpiresAt = expiresAt;

        new SessionEntityValidator().ValidateAndThrow(this);
    }
    
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}