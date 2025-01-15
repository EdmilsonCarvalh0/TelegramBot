using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Core;

namespace TelegramBot;

public class Program
{
    static async Task Main(string[] args)
    {
        ItemController client = new ItemController();

        using var cts = new CancellationTokenSource();

        // Receiver configurations
        client.Bot.StartReceiving(
            BotConnection.HandleUpdateAsync,
            BotConnection.HandleErrorAsync,
            new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            },
            cancellationToken: cts.Token
        );

        //Exibe informações do bot
        var me = await client.Bot.GetMe();
        Console.WriteLine($"Bot iniciado: @{me.Username}");
        Console.ReadLine();

        //Encerra o bot
        cts.Cancel();
    }
}