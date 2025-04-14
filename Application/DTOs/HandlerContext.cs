using TelegramBot.Domain.Item;
using TelegramBot.Infrastructure;
using TelegramBot.Service;

namespace TelegramBot.Application;

public class HandlerContext
{
    public readonly UserStateManager StateManager;
    public readonly IItemRepository ItemRepository;
    public readonly ShoppingAssistantMode ShoppingAssistant;
    public IInputItemService InputItemService;
    public BotRequestContext? Context;

    
    public HandlerContext(UserStateManager stateManager, IItemRepository itemRepository, ShoppingAssistantMode shoppingAssistant, IInputItemService inputItemService)
    {
        StateManager = stateManager;
        ItemRepository = itemRepository;
        ShoppingAssistant = shoppingAssistant;
        InputItemService = inputItemService;
    }
}