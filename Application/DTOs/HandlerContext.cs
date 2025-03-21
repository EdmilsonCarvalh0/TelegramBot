using TelegramBot.Infrastructure;

namespace TelegramBot.Application;

public class HandlerContext
{
    public UserStateManager? StateManager;
    public IItemRepository? ItemRepository;
    public BotRequestContext? Context;

    
    public void SetUpdateContext(UserStateManager stateManager, IItemRepository itemRepository, BotRequestContext context)
    {
        StateManager = stateManager;
        ItemRepository = itemRepository;
        Context = context;
    }
}