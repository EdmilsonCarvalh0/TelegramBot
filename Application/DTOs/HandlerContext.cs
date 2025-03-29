using TelegramBot.Infrastructure;
using TelegramBot.Service;

namespace TelegramBot.Application;

public class HandlerContext
{
    public UserStateManager StateManager;
    public IItemRepository ItemRepository;
    public ShoppingAssistantMode ShoppingAssistant;
    public BotRequestContext? Context;

    
    public HandlerContext(UserStateManager stateManager, IItemRepository itemRepository, ShoppingAssistantMode shoppingAssistant)
    {
        StateManager = stateManager;
        ItemRepository = itemRepository;
        ShoppingAssistant = shoppingAssistant;
    }
}