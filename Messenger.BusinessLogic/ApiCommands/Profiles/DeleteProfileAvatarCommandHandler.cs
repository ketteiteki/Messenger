using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.BusinessLogic.Services;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Profiles;

public class DeleteProfileAvatarCommandHandler : IRequestHandler<DeleteProfileAvatarCommand, Result<UserDto>>
{
    private readonly DatabaseContext _context;
    private readonly IFileService _fileService;

    public DeleteProfileAvatarCommandHandler(DatabaseContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<Result<UserDto>> Handle(DeleteProfileAvatarCommand request, CancellationToken cancellationToken)
    {
        var requester = await _context.Users.FirstAsync(u => u.Id == request.RequesterId, CancellationToken.None);

        if (requester.AvatarLink == null) return new Result<UserDto>(new ForbiddenError("Avatar not exists"));

        _fileService.DeleteFile(Path.Combine(BaseDirService.GetPathWwwRoot(), requester.AvatarLink.Split("/")[^1]));
        requester.AvatarLink = null;
		
        await _context.SaveChangesAsync(cancellationToken);

        return new Result<UserDto>(new UserDto(requester));
    }
}