using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.Domain.Entities;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.DeleteConversationCommandHandlerTests;

public class DeleteConversationTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21th = EntityHelper.CreateUser21th();
		
		DatabaseContextFixture.Users.AddRange(user21th);
		await DatabaseContextFixture.SaveChangesAsync();

		var conversation = EntityHelper.CreateConversation(user21th.Id, "qwerty", "qwerty");
		
		DatabaseContextFixture.Chats.Add(conversation);
		await DatabaseContextFixture.SaveChangesAsync();
		
		DatabaseContextFixture.ChatUsers.Add(new ChatUser {UserId = user21th.Id, ChatId = conversation.Id});
		await DatabaseContextFixture.SaveChangesAsync();

		var deleteConversationCommand = new DeleteConversationCommand(
			RequestorId: user21th.Id,
			ChatId: conversation.Id);

		await MessengerModule.RequestAsync(deleteConversationCommand, CancellationToken.None);

		var conversationForCheck = await DatabaseContextFixture.Chats
			.FirstOrDefaultAsync(c => c.Id == conversation.Id);

		conversationForCheck.Should().BeNull();
	}
}