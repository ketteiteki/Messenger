namespace Messenger.Domain.Entities;

public class UserSessionEntity
{
    public Guid Id { get; private set; }
    
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    
    public DateTimeOffset ExpiresAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset DateOfLastAccess { get; private set; } = DateTimeOffset.UtcNow;
    
    public byte[] Value { get; private set; }
    
    public Guid UserId { get; set; }
    
    public UserEntity User { get; set; }

    public UserSessionEntity(Guid id, Guid userId, DateTimeOffset expiresAt, byte[] value)
    {
        Id = id;
        ExpiresAt = expiresAt;
        Value = value;
        UserId = userId;
    }

    public void UpdateValue(byte[] value)
    {
        Value = value;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
    
    public void UpdateExpiresAt(DateTimeOffset expiresAt)
    {
        ExpiresAt = expiresAt;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
    
    public void UpdateDateOfLastAccess(DateTimeOffset dateOfLastAccess)
    {
        DateOfLastAccess = dateOfLastAccess;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}