using TelegramBot.Infrastructure;

namespace TelegramBot.Application;

public class HandlerContext
{
    public UserStateManager StateManager;
    public IItemRepository ItemRepository;
    public BotRequestContext? Context;

    
    public HandlerContext(UserStateManager stateManager, IItemRepository itemRepository)
    {
        StateManager = stateManager;
        ItemRepository = itemRepository;
    }
}