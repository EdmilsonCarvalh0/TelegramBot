using System.Data.Common;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Data;

namespace TelegramBot.Entities;

//TODO: Create Class about List/Data for encapsulate functionalities
//      criar classe sobre list/dados para encapsular funcionalidades
public class BotClient : IBotClient
{
    private readonly static string Token = "7560368958:AAGSWm6chmVviBNYSNF8P4Yh3aJdcka0vQw";
    public TelegramBotClient Bot;
    private static readonly IShoppingData _shoppingData = new ShoppingData();

    public BotClient()
    {
        Bot = new TelegramBotClient(Token);
    }

    public InlineKeyboardMarkup StartService()
    {
        return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Ver lista"),
                    InlineKeyboardButton.WithCallbackData("Atualizar lista"),
                    InlineKeyboardButton.WithCallbackData("Criar nova lista")
                }
            }
        );
    }

    public InlineKeyboardMarkup GetOptionsOfListUpdate()
    {
        return new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Adicionar um item"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Alterar um item")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Remover um item")
                }
            }
        );
    }

    public InlineKeyboardMarkup GetAttributeOptions()
    {
        return new InlineKeyboardMarkup(new[]
            {  
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Nome"),
                    InlineKeyboardButton.WithCallbackData("Marca"),
                    InlineKeyboardButton.WithCallbackData("Preço")
                }
            }
        );
    }

    public string AddItemInShoppingData(string userItems)
    {
        _shoppingData.AddItemInList(userItems);
        return $"{userItems} adicionado(a).";
    }

    public string SendItemToUpdateList(string item)
    {
        _shoppingData.UpdateList(item);
        return "Lista atualizada.";
    }

    public string SendItemToRemoveFromList(string item)
    {
        _shoppingData.RemoveItemFromList(item);
        return "Item removido.";
    }
    
    public string ShowList()
    {
        var list = _shoppingData.GetList();
        return list;
    }

    public string GetItemsToCreatelist(string items)
    {
        //TODO: manipulate TimeStamp to formalize data
        _shoppingData.CreateNewList(items);
        return "Nova lista criada.";
    }

    //TODO: implement verify function automatic of item
}