using BorClone.Entity;
using BorClone.Resources;
using BorClone.Services;
using Microsoft.Extensions.Localization;
using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = BorClone.Entity.User;

namespace BorClone.Services;

public partial class BotUpdateHandler : IUpdateHandler
{
    private readonly ILogger<BotUpdateHandler> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private  IStringLocalizer _localizer;
    private UserService _userService;

    public BotUpdateHandler(
        ILogger<BotUpdateHandler> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Error occured with telegram bot: {exception.Message}", exception.Message);
        return Task.CompletedTask;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        _userService = scope.ServiceProvider.GetRequiredService<UserService>();

        var culture = await GetCultureForUser(update);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        _localizer = scope.ServiceProvider.GetService<IStringLocalizer<BotLocalizer>>();



        var handler = update.Type switch
        {
            UpdateType.Message => HandleMessageAsync(botClient, update.Message, cancellationToken),
            UpdateType.EditedMessage => HandleEditedMessageAsync(botClient, update.EditedMessage, cancellationToken),
            // handle other updates
            _ => HandleUnknownUpdate(botClient, update, cancellationToken)
        };

        try
        {
            await handler;
        }
        catch (Exception e)
        {

            await HandlePollingErrorAsync(botClient, e, cancellationToken);
        }

    }

    private async Task<CultureInfo> GetCultureForUser(Update update)
    {
        Telegram.Bot.Types.User? from = update.Type switch
        {
            UpdateType.Message => update?.Message?.From,
            UpdateType.EditedMessage => update?.EditedMessage?.From,
            UpdateType.CallbackQuery => update?.CallbackQuery?.From,
            UpdateType.InlineQuery => update?.InlineQuery?.From,
            _ => update?.Message?.From
        };
        var user = await _userService.GetUserAsync(from?.Id);

        return new CultureInfo(user?.LanguageCode ?? "uz-Uz");
        
    }

    private Task HandleUnknownUpdate(ITelegramBotClient client, Update update, CancellationToken token)
    {
        _logger.LogInformation("Update type {update.Type} is recieved", update.Type);
        return Task.CompletedTask;
    }

   
}

