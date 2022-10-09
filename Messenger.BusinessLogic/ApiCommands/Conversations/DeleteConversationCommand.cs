using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public record DeleteConversationCommand(
	Guid ChatId,
	Guid RequestorId) 
	: IRequest<Result<ChatDto>>;