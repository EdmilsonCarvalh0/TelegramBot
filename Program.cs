using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Entities;

namespace TelegramBot;

public class Program
{
    static async Task Main(string[] args)
    {
        MessageHandler client = new MessageHandler();

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


        // while(true)
        // {
        //     //TODO: implement BotClient methods to handle options
        //     //      implementar os métodos de BotClient pra manusear as opções
        //     var updates = await client.Bot.GetUpdates();

        //     foreach (var update in updates)
        //     {
        //         if(update.Message != null)
        //         {
        //             var chatId = update.Message.Chat.Id;
        //             var messageText = update.Message.Text;

        //             Console.WriteLine($"Mensagem recebida: {messageText}");

        //             await client.Bot.SendMessage(chatId, $"Você disse: {messageText}");
        //         }
        //     }

        //     await Task.Delay(1000);
        // }

    
    }

    

    
}