using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Channels;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.ApiQueries.Channels;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetChannelQueryHandlerTests;

public class GetChannelTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
		var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);
		var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);

		var createChannelCommand = new CreateChannelCommand(
			RequesterId: user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			AvatarFile: null);

		var channel = await MessengerModule.RequestAsync(createChannelCommand, CancellationToken.None);

		await MessengerModule.RequestAsync(
			new JoinToChatCommand(
				RequesterId: bob.Value.Id,
				ChatId: channel.Value.Id), CancellationToken.None);

		var createPermissionsForBob = new CreatePermissionsUserInConversationCommand(
			RequesterId: user21Th.Value.Id,
			UserId: bob.Value.Id,
			ChatId: channel.Value.Id,
			CanSendMedia: false);

		await MessengerModule.RequestAsync(createPermissionsForBob, CancellationToken.None);
		
		var channelForRequesterQuery = new GetChannelQuery(
			RequesterId: user21Th.Value.Id,
			ChannelId: channel.Value.Id);
		var channelForAliceQuery = new GetChannelQuery(
			RequesterId: alice.Value.Id,
			ChannelId: channel.Value.Id);
		var channelForBobQuery = new GetChannelQuery(
			RequesterId: bob.Value.Id,
			ChannelId: channel.Value.Id);

		var chatForRequester = await MessengerModule.RequestAsync(channelForRequesterQuery, CancellationToken.None);
		var chatForAlice = await MessengerModule.RequestAsync(channelForAliceQuery, CancellationToken.None);
		var chatForBob = await MessengerModule.RequestAsync(channelForBobQuery, CancellationToken.None);

		chatForRequester.Value.IsMember.Should().Be(true);
		chatForRequester.Value.IsOwner.Should().Be(true);
		chatForRequester.Value.MembersCount.Should().Be(2);
		chatForRequester.Value.CanSendMedia.Should().Be(true);
		
		chatForAlice.Value.IsMember.Should().Be(false);
		chatForAlice.Value.IsOwner.Should().Be(false);
		chatForAlice.Value.MembersCount.Should().Be(2);
		chatForAlice.Value.CanSendMedia.Should().Be(false);
		
		chatForBob.Value.IsMember.Should().Be(true);
		chatForBob.Value.IsOwner.Should().Be(false);
		chatForBob.Value.MembersCount.Should().Be(2);
		chatForBob.Value.CanSendMedia.Should().Be(false);
	}
}