using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Entities;

namespace TelegramBot;

public class Program
{
    static async Task Main(string[] args)
    {
        BotClient client = new BotClient();

        while(true)
        {
            //TODO: implement BotClient methods to handle options
            //      implementar os métodos de BotClient pra manusear as opções
            var updates = await client.Bot.GetUpdates();

            foreach (var update in updates)
            {
                if(update.Message != null)
                {
                    var chatId = update.Message.Chat.Id;
                    var messageText = update.Message.Text;

                    Console.WriteLine($"Mensagem recebida: {messageText}");

                    await client.Bot.SendMessage(chatId, $"Você disse: {messageText}");
                }
            }

            await Task.Delay(1000);
        }

    
    }

    // public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    // {
    //     if (update.Type == UpdateType.Message && update.Message?.Text != null)
    //     {

    //     }
    // }
}