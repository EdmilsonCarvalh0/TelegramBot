using Telegram.Bot;
using TelegramBot.Application;

namespace TelegramBot.Infrastructure;


public class MessageSender
{
    private readonly ITelegramBotClient _botClient;

    public MessageSender(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task SendMessageAsync(ResponseContentDTO responseContent, BotRequestContext context)
    {
        if (responseContent.KeyboardMarkup != null)
        {
            await _botClient.SendMessage(
                chatId: context.UserId,
                text: responseContent.Text,
                replyMarkup: responseContent.KeyboardMarkup,
                cancellationToken: context.CancellationToken
            );

            UserStateManager.SetState(context.UserId, responseContent.UserState);
            return;
        }

        await _botClient.SendMessage(
            chatId: context.UserId,
            text: responseContent.Text,
            cancellationToken: context.CancellationToken
        );

        UserStateManager.SetState(context.UserId, responseContent.UserState);
        return;
    }
}