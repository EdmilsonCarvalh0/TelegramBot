using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TelegramBot.Application;

namespace TelegramBot.Infrastructure;


public class MessageSender
{
    private readonly ITelegramBotClient _botClient;
    private readonly ChatIdIdentifier _chatId;

    public MessageSender(ITelegramBotClient botClient, ChatIdIdentifier chatId)
    {
        _botClient = botClient;
        _chatId = chatId;
    }

    public async Task SendMessageAsync(ResponseContent responseContent, CancellationToken cancellationToken)
    {
        await _botClient.SendMessage(
            chatId: _chatId.BotId,
            text: responseContent.Text,
            replyMarkup: responseContent.KeyboardMarkup,
            cancellationToken: cancellationToken
        );

        UserStateManager.SetState(_chatId.BotId, responseContent.UserState); // novo state precisa ser definido aqui?
    }
}