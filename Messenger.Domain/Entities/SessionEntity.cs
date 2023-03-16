using FluentValidation;
using Messenger.Domain.Entities.Abstraction;
using Messenger.Domain.Entities.Validation;

namespace Messenger.Domain.Entities;

public class SessionEntity : IBaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid UserId { get; set; }

    public UserEntity User { get; set; }

    public string AccessToken { get; set; }

    public Guid RefreshToken { get; set; } = Guid.NewGuid();

    public string Ip { get; set; }
    
    public string UserAgent { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime CreateAt { get; set; } = DateTime.UtcNow;

    public SessionEntity(Guid userId, string accessToken, string ip, string userAgent, DateTime expiresAt)
    {
        AccessToken = accessToken;
        UserId = userId;
        Ip = ip;
        UserAgent = userAgent;
        ExpiresAt = expiresAt;

        new SessionEntityValidator().ValidateAndThrow(this);
    }

    public void UpdateAccessToken(string accessToken)
    {
        AccessToken = accessToken;
        new SessionEntityValidator().ValidateAndThrow(this);
    }
    
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}