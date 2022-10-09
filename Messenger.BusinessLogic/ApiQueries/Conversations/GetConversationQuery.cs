using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiQueries.Conversations;

public record GetConversationQuery(
		Guid RequestorId,
		Guid ChatId)
	: IRequest<Result<ChatDto>>;