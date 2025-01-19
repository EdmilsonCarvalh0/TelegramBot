using System.Data.Common;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Service;

namespace TelegramBot.UserInterface;

//TODO: Create Class about List/Data for encapsulate functionalities
//      criar classe sobre list/dados para encapsular funcionalidades
public class ItemController : IItemController
{
    private readonly static string Token = "7560368958:AAGSWm6chmVviBNYSNF8P4Yh3aJdcka0vQw";
    public TelegramBotClient Bot;
    private static readonly IItemRepository _itemRepository = new JsonItemRepository();

    public ItemController()
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

    public string CheckItemExistence(string nameAttribute)
    {
        var response = _itemRepository.GetItemInRepository(nameAttribute);

        //if (response.Contains('\n'))
        //{
        //    return $"Encontrei os seguintes itens:\n{response}\nQual deles você quer alterar?";
        //}

        return response.Contains('\n') ? $"Encontrei os seguintes itens:\n\n{response}\nQual deles você quer alterar?" : response;
    }

    public string AddItemInShoppingData(string userItems)
    {
        _itemRepository.AddItemInList(userItems);
        return $"{userItems} adicionado(a).";
    }

    public string SendItemToUpdateList(string item)
    {
        _itemRepository.UpdateList(item);
        return "Pronto, alterei pra você.";
    }

    public string SendItemToRemoveFromList(string item)
    {
        _itemRepository.RemoveItemFromList(item);
        return "Item removido.";
    }
    
    public string ShowList()
    {
        var list = _itemRepository.GetList();
        return list;
    }

    public string GetItemsToCreatelist(string items)
    {
        //TODO: manipulate TimeStamp to formalize data
        _itemRepository.CreateNewList(items);
        return "Nova lista criada.";
    }

    //TODO: implement verify function automatic of item
}