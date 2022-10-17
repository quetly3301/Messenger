using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.BusinessLogic.Services;
using Messenger.Domain.Constants;
using Messenger.Services;
using Microsoft.Extensions.Configuration;

namespace Messenger.BusinessLogic.ApiCommands.Profiles;

public class UpdateProfileAvatarCommandHandler : IRequestHandler<UpdateProfileAvatarCommand, Result<UserDto>>
{
	private readonly DatabaseContext _context;
	private readonly IFileService _fileService;
	private readonly IConfiguration _configuration;

	public UpdateProfileAvatarCommandHandler(DatabaseContext context, IFileService fileService, IConfiguration configuration)
	{
		_context = context;
		_fileService = fileService;
		_configuration = configuration;
	}
	
	public async Task<Result<UserDto>> Handle(UpdateProfileAvatarCommand request, CancellationToken cancellationToken)
	{
		var user = await _context.Users.FindAsync(request.RequesterId);

		if (user == null) return new Result<UserDto>(new DbEntityNotFoundError("User not found")); 

		if (user.AvatarLink != null)
		{
			_fileService.DeleteFile(Path.Combine(BaseDirService.GetPathWwwRoot(), user.AvatarLink.Split("/")[^1]));
			user.AvatarLink = null;
		}

		if (request.AvatarFile != null)
		{
			var avatarLink = await _fileService.CreateFileAsync(BaseDirService.GetPathWwwRoot(), request.AvatarFile,
				_configuration[AppSettingConstants.MessengerDomainName]);

			user.AvatarLink = avatarLink;
		}

		_context.Users.Update(user);
		await _context.SaveChangesAsync(cancellationToken);

		return new Result<UserDto>(new UserDto(user));
	}
}