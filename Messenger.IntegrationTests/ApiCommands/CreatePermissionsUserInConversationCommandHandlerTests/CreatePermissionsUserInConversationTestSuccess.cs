using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.Domain.Enums;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.CreatePermissionsUserInConversationCommandHandlerTests;

public class CreatePermissionsUserInConversationTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
		var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
		var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);
		var alex = await MessengerModule.RequestAsync(CommandHelper.RegistrationAlexCommand(), CancellationToken.None);

		var createConversationCommand = new CreateChatCommand(
			user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			ChatType.Conversation,
			AvatarFile: null);

		var createConversationResult = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

		var addAliceInConversationBy21ThCommand = new AddUserToConversationCommand(
			user21Th.Value.Id,
			createConversationResult.Value.Id,
			alice.Value.Id);
		
		var addBobInConversationBy21ThCommand = new AddUserToConversationCommand(
			user21Th.Value.Id,
			createConversationResult.Value.Id,
			bob.Value.Id);
		
		var addAlexInConversationBy21ThCommand = new AddUserToConversationCommand(
			user21Th.Value.Id,
			createConversationResult.Value.Id,
			alex.Value.Id);

		await MessengerModule.RequestAsync(addAliceInConversationBy21ThCommand, CancellationToken.None);
		await MessengerModule.RequestAsync(addBobInConversationBy21ThCommand, CancellationToken.None);
		await MessengerModule.RequestAsync(addAlexInConversationBy21ThCommand, CancellationToken.None);
		
		var createAliceRoleBy21ThCommand = new CreateOrUpdateRoleUserInConversationCommand(
			user21Th.Value.Id,
			createConversationResult.Value.Id,
			alice.Value.Id,
			RoleTitle: "moderator",
			RoleColor.Blue,
			CanBanUser: false,
			CanChangeChatData: false,
			CanAddAndRemoveUserToConversation: false,
			CanGivePermissionToUser: true);

		await MessengerModule.RequestAsync(createAliceRoleBy21ThCommand, CancellationToken.None);
		
		var createPermissionsBobInConversationBy21ThCommand = new CreatePermissionsUserInConversationCommand(
			user21Th.Value.Id,
			createConversationResult.Value.Id,
			bob.Value.Id,
			CanSendMedia: false,
			MuteMinutes: 5);

		var createPermissionsAlexInConversationByAliceCommand = new CreatePermissionsUserInConversationCommand(
			RequesterId: alice.Value.Id,
			UserId: alex.Value.Id,
			ChatId: createConversationResult.Value.Id,
			CanSendMedia: false,
			MuteMinutes: null);
		
		var createPermissionsUserInConversationBy21ThResult =
			await MessengerModule.RequestAsync(createPermissionsBobInConversationBy21ThCommand, CancellationToken.None);
		
		var createPermissionsUserInConversationByAliceResult = 
			await MessengerModule.RequestAsync(createPermissionsAlexInConversationByAliceCommand, CancellationToken.None);
		
		createPermissionsUserInConversationBy21ThResult.Value.CanSendMedia.Should().BeFalse();
		createPermissionsUserInConversationBy21ThResult.Value.MuteDateOfExpire.Should().NotBeNull();
		createPermissionsUserInConversationByAliceResult.Value.CanSendMedia.Should().BeFalse();
	}
}