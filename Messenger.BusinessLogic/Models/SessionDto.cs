using Messenger.Domain.Entities;

namespace Messenger.BusinessLogic.Models;

public class SessionDto
{
    public Guid Id { get; set; }

    public string Ip { get; set; }
    
    public string UserAgent { get; set; }
    
    public DateTime CreateAt { get; set; }

    public SessionDto(SessionEntity session)
    {
        Id = session.Id;
        Ip = session.Ip;
        UserAgent = session.UserAgent;
        CreateAt = session.CreateAt;
    }
}