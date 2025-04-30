using Newtonsoft.Json;
using TelegramBot.Domain.Item;

namespace TelegramBot.DataModels.Item;

public class ItemDataFormatter
{
    [JsonProperty("items")]
    public List<global::TelegramBot.Domain.Item.Item> Items { get; set; } = [];

    public global::TelegramBot.Domain.Item.Item? GetItemInList(string itemName)
    {
        return Items.FirstOrDefault(
            item => item.Nome.Value.Equals(itemName, StringComparison.CurrentCultureIgnoreCase));
    }

    public void SequentializeIDs()
    {
        if(Items.Count <= 1) return;
        
        List<global::TelegramBot.Domain.Item.Item> temporaryList = Items;
        temporaryList[0].Id.Value = 1;

        for (int i = 0; i < temporaryList.Count -1; i++)
        {
            temporaryList[i+1].Id.Value = temporaryList[i+1].Id.Value - temporaryList[i].Id.Value > 1 ? temporaryList[i].Id.Value+1 : temporaryList[i+1].Id.Value;
        }

        Items = temporaryList;
    }
}