using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Messages;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enums;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.CreateMessageCommandHandlerTests;

public class CreateMessageWithAttachmentTestThrowForbidden : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        var createConversationCommand = new CreateChatCommand(
            user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            ChatType.Conversation,
            AvatarFile: null);
		
        var conversation = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

        var createMessageBy21ThCommand = new CreateMessageCommand(
            user21Th.Value.Id,
            Text: "qwerty1",
            ReplyToId: null,
            conversation.Value.Id,
            Files: new List<IFormFile>
            {
                FilesHelper.GetFile(),
                FilesHelper.GetFile(),
                FilesHelper.GetFile(),
                FilesHelper.GetFile(),
                FilesHelper.GetFile()
            });

        var createMessageBy21ThResult = await MessengerModule.RequestAsync(createMessageBy21ThCommand, CancellationToken.None);

        createMessageBy21ThResult.Error.Should().BeOfType<ForbiddenError>();
    }
}