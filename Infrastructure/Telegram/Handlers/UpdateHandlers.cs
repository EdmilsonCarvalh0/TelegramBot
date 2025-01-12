

using Telegram.Bot;
using Telegram.Bot.Types;

public class UpdateHandlers
{
    public async Task HandleMessageAsync(BotRequestContext context)
    {
        if (context.Message?.Text == null)
        {
            await context.BotClient.SendMessage(
                context.UserId,
                "Mensagem inválida. Por favor, tente novamente.",
                cancellationToken: context.CancellationToken
            );
            return;
        }


    }

    public async Task HandleCallbackQueryAsync(BotRequestContext context)
    {
        if (context.CallbackQuery == null) return;

        
    }
}