using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace BorClone.Services;

public partial class BotUpdateHandler
{
    private async Task HandleMessageAsync(ITelegramBotClient client, Message? message, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(message);

        var from = message.From;
        _logger.LogInformation("Recieved message from {from.FirstName}: {message.Text}", from?.FirstName, message.Text);

        var handler = message.Type switch
        {
            MessageType.Text => HandleTextMessageAsync(client, message, token),
            _ => HandleUnknownMessageAsync(client, message, token)
        };
        await handler;
    }

    private Task HandleUnknownMessageAsync(ITelegramBotClient client, Message message, CancellationToken token)
    {
        _logger.LogInformation("Recieved message type {message.Type}", message.Type);

        return Task.CompletedTask;
    }

    private async Task HandleTextMessageAsync(ITelegramBotClient client, Message message, CancellationToken token)
    {
        var from = message.From;
        _logger.LogInformation("From: {from.FirstName}", from?.FirstName);
        await client.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: _localizer["choose-language"],
            replyToMessageId: message.MessageId,
            cancellationToken: token);
    }
}

