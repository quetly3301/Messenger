using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.Conversations.Commands;

public class UpdateConversationAvatarCommandHandler : IRequestHandler<UpdateConversationAvatarCommand, ChatDto>
{
	private readonly DatabaseContext _context;
	private readonly IFileService _fileService;
	private readonly IWebHostEnvironment _webHostEnvironment;
	
	public UpdateConversationAvatarCommandHandler(DatabaseContext context, IFileService fileService,
		IWebHostEnvironment webHostEnvironment)
	{
		_context = context;
		_fileService = fileService;
		_webHostEnvironment = webHostEnvironment;
	}
	
	public async Task<ChatDto> Handle(UpdateConversationAvatarCommand request, CancellationToken cancellationToken)
	{
		var chatUserByRequester = await _context.ChatUsers
			.FirstOrDefaultAsync(r => r.UserId == request.RequesterId && r.ChatId == request.ChatId, cancellationToken);

		if (chatUserByRequester == null)
			throw new DbEntityNotFoundException("No requester in the chat");
		
		if (chatUserByRequester.Role is { CanChangeChatData: true } || 
		    chatUserByRequester.Chat.OwnerId == request.RequesterId)
		{
			if (chatUserByRequester.Chat.AvatarLink != null)
			{
				_fileService.DeleteFile(Path.Combine(_webHostEnvironment.WebRootPath,
					chatUserByRequester.Chat.AvatarLink.Split("/")[^1]));
				chatUserByRequester.Chat.AvatarLink = null;
			}
			
			if (request.AvatarFile != null)
			{
				var avatarLink = await _fileService.CreateFileAsync(_webHostEnvironment.WebRootPath, request.AvatarFile);
				chatUserByRequester.Chat.AvatarLink = avatarLink;
			}
			
			_context.Chats.Update(chatUserByRequester.Chat);
			await _context.SaveChangesAsync(cancellationToken);
			
			return new ChatDto
			{
				Id = chatUserByRequester.ChatId,
				Name = chatUserByRequester.Chat.Name,
				Title = chatUserByRequester.Chat.Title,
				Type = chatUserByRequester.Chat.Type,
				AvatarLink = chatUserByRequester.Chat.AvatarLink,
				IsOwner = chatUserByRequester.Chat.OwnerId == request.RequesterId,
				IsMember = true,
				MuteDateOfExpire = chatUserByRequester.MuteDateOfExpire,
				BanDateOfExpire = null,
				RoleUser = chatUserByRequester.Role != null ? new RoleUserByChatDto(chatUserByRequester.Role) : null
			};
		}
		
		throw new ForbiddenException("It is forbidden to update someone else's conversation");
	}
}