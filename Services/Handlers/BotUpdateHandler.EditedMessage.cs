using Telegram.Bot;
using Telegram.Bot.Types;

namespace BorClone.Services;

public partial class BotUpdateHandler
{
    private async Task HandleEditedMessageAsync(ITelegramBotClient client, Message? message, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(message);

        var from = message.From;

        _logger.LogInformation("Edited message from {from.FirstName}, {message.Text}", from?.FirstName, message.Text);
    }
}