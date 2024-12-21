using System.Data.Common;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Data;

namespace TelegramBot.Entities;

public class BotClient
{
    private readonly static string Token = "7560368958:AAGSWm6chmVviBNYSNF8P4Yh3aJdcka0vQw";
    public TelegramBotClient Bot;
    private static ShoppingData _shoppingData = new ShoppingData();
    private static string InputMessage = "";

    public BotClient()
    {
        Bot = new TelegramBotClient(Token);
    }

    public static InlineKeyboardMarkup StartService()
    {
        //TODO: send the mesage with the user click option
        //      enivar a mensagem com opção de clique pelo usuário
        var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Ver lista"),
                    InlineKeyboardButton.WithCallbackData("Atualizar lista")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Adicionar item"),
                    InlineKeyboardButton.WithCallbackData("Criar nova lista")
                }
            }
        );
        return inlineKeyboard;
    }

    public static string ExecuteChosenOption(string chosenOption)
    {
        switch (chosenOption)
        {
            case "Ver lista":
                return _shoppingData.GetList();

            case "Atualizar lista":
                _shoppingData.UpdateList();
                return "Lista atualizada";
            
            case "Adicionar item":
                string item = Console.ReadLine() ?? "Manteiga";
                ValidateInputItem(item);
                _shoppingData.AddItem(item);
                //TODO: implement gender verification of items
                //      implementar verificação de sexo dos itens
                return $"{item} adicionado(a)!";

            case "Criar nova lista":
                return "Nova lista criada!";
            
            default:
                return "Switch final.";
        }
    }

    public static void SetInputMessage(string inputItem)
    {
        InputMessage = inputItem;
    }

    public static void ValidateInputItem(string input)
    {
        if(String.IsNullOrEmpty(input))
        {
            throw new ArgumentException("Por favor, informe uma opção ou um item válido.");
        }
    }
}