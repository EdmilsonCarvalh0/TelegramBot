using TelegramBot.Infrastructure;
using TelegramBot.Service.ItemRepository.Interface;
using TelegramBot.Service.ShoppingAssistant;
using TelegramBot.UserInterface.Interfaces;

namespace TelegramBot.Application.DTOs;

public class HandlerContext
{
    public readonly UserStateManager StateManager;
    public readonly IItemRepository ItemRepository;
    public readonly ShoppingAssistantMode ShoppingAssistant;
    public readonly IKeyboardButtonFactory KeyboardFactory;
    public BotRequestContext? Context;
    
    public HandlerContext(UserStateManager stateManager, IItemRepository itemRepository,
                        ShoppingAssistantMode shoppingAssistant, IKeyboardButtonFactory keyboardFactory)
    {
        StateManager = stateManager;
        ItemRepository = itemRepository;
        ShoppingAssistant = shoppingAssistant;
        KeyboardFactory = keyboardFactory;
    }
}