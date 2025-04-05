using Domain.Item;
using TelegramBot.Data;

namespace TelegramBot.Application;

public class ItemFactory
{
    public Item Create(int id, string name, string brand, decimal price)
    {
        return new Item(
            new ItemId(id),
            new Name(name),
            new Brand(brand),
            new Price(price)
        );
    }
}