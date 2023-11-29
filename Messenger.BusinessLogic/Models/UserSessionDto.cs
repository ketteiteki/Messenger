namespace Messenger.BusinessLogic.Models;

public class UserSessionDto
{
    public Guid Id { get; set; }
    
    public DateTimeOffset CreateAt { get; private set; }

    public UserSessionDto(Guid id, DateTimeOffset createAt)
    {
        Id = id;
        CreateAt = createAt;
    }
}