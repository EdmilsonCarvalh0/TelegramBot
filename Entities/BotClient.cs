using System.Data.Common;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Data;

namespace TelegramBot.Entities;

public class BotClient
{
    private readonly static string Token = "7560368958:AAGSWm6chmVviBNYSNF8P4Yh3aJdcka0vQw";
    public TelegramBotClient Bot;
    private readonly long Id = 1322812622;
    private ShoppingData _shoppingData = new ShoppingData();
    private static string InputMessage = "";

    public BotClient()
    {
        Bot = new TelegramBotClient(Token);
    }

    public async void ShowOptions()
    {
        //TODO: send the mesage with the user click option
        //      enivar a mensagem com opção de clique pelo usuário
        await Bot.SendMessage(Id, "");
    }

    public async void ExecuteChosenOption(string chosenOption)
    {
        switch (chosenOption)
        {
            case "/verLista":
                await Bot.SendMessage(Id, _shoppingData.GetList());
                break;

            case "/atualizarLista":
                _shoppingData.UpdateList();
                await Bot.SendMessage(Id, _shoppingData.GetList());
                break;
            
            case "/adicionarItem":
                string item = Console.ReadLine()!;
                ValidateInputItem(item);
                _shoppingData.AddItem(item);
                break;

            case "/criarNovaLista":

                break;
        }
    }

    public static void GetInputMessage(string inputItem)
    {
        InputMessage = inputItem;
    }

    public void ValidateInputItem(string input)
    {
        if(String.IsNullOrEmpty(input))
        {
            throw new ArgumentException("Por favor, informe uma opção ou uma item válido.");
        }
    }

    
}