using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.Domain.Enums;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.JoinToChatCommandHandlerTests;

public class JoinToChatTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
		var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
		
		var createConversationCommand = new CreateChatCommand(
			user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			ChatType.Conversation,
			AvatarFile: null);

		var createConversationResult = 
			await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);
		
		var aliceJoinToConversationCommand = new JoinToChatCommand(alice.Value.Id, createConversationResult.Value.Id);

		var joinToConversation = 
			await MessengerModule.RequestAsync(aliceJoinToConversationCommand, CancellationToken.None);

		joinToConversation.IsSuccess.Should().BeTrue();
	}
}