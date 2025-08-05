using Application.Handlers.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Application.Bot;

namespace TelegramBot.Infrastructure.Telegram;

public class BotConnection
{
    private readonly ITelegramBotClient _bot;
    private readonly IUpdateHandlers _handlers;
    private readonly IResponseManager _responseManager;
    private readonly IMessageSender _messageSender;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly IServiceScopeFactory _scopeFactory;

    public BotConnection(ITelegramBotClient bot, IUpdateHandlers handlers, IResponseManager responseManager, 
                        IMessageSender messageSender, IHostApplicationLifetime appLifetime, IServiceScopeFactory scopeFactory)
    {
        _bot = bot;
        _handlers = handlers;
        _responseManager = responseManager;
        _messageSender = messageSender;
        _appLifetime = appLifetime;
        _scopeFactory = scopeFactory;
    }

    public void Start()
    {
        _bot.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            },
            cancellationToken: _appLifetime.ApplicationStopping
        );
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var context = CreateContext(update, cancellationToken);
        try
        {
            var response = ProcessUpdate(context);
            await SendResponseAsync(response, context.UserId, cancellationToken);
        }

        catch (Exception e)
        {
            await HandleExceptionAsync(e, context.UserId, cancellationToken);
        }
    }

    private BotRequestContext CreateContext(Update update, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var contextFactory = scope.ServiceProvider.GetRequiredService<BotRequestContextFactory>();
        long userId = update.CallbackQuery?.From.Id ?? update.Message?.Chat.Id ?? 0;
        return contextFactory.Create(userId, update);
    }

    private ResponseContent ProcessUpdate(BotRequestContext context)
    {
        var responseInfo = _handlers.DelegateUpdates(context);
        return _responseManager.ProcessResponse(responseInfo);
    }

    private async Task SendResponseAsync(ResponseContent response, long userId, CancellationToken cancellationToken)
    {
        await _messageSender.SendMessageAsync(response, userId, cancellationToken);
    }

    private async Task HandleExceptionAsync(Exception e, long userId, CancellationToken cancellationToken)
    {
        ResponseContent responseContent = new ResponseContent();
        responseContent.Text = e.ToString();
        await _messageSender.SendMessageAsync(responseContent, userId, cancellationToken);
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Erro recebido: {exception.Message}\nData: {exception.StackTrace}");
        return Task.CompletedTask;
    }

    public async Task DisplayBotInfoAsync()
    {
        var me = await _bot.GetMe();
        Console.WriteLine($"\n--> Bot iniciado: @{me.Username} <--\n");
    }
}
