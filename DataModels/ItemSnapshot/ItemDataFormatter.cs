using Newtonsoft.Json;
using TelegramBot.Domain.ItemEntity;

namespace TelegramBot.DataModels.ItemSnapshot;

public class ItemDataFormatter
{
    [JsonProperty("items")]
    public List<Item> Items { get; set; } = [];

    public Item? GetItemInList(string itemName)
    {
        return Items.FirstOrDefault(
            item => item.Nome.Value.Equals(itemName, StringComparison.CurrentCultureIgnoreCase));
    }

    public void SequentializeIDs()
    {
        if(Items.Count <= 1) return;
        
        List<Item> temporaryList = Items;
        temporaryList[0].Id.Value = 1;

        for (int i = 0; i < temporaryList.Count -1; i++)
        {
            temporaryList[i+1].Id.Value = temporaryList[i+1].Id.Value - temporaryList[i].Id.Value > 1 ? temporaryList[i].Id.Value+1 : temporaryList[i+1].Id.Value;
        }

        Items = temporaryList;
    }
}