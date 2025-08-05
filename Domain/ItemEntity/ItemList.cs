namespace TelegramBot.Domain.ItemEntity;

// VERIFICAR ONDE MAIS UTILIZAR A CLASSE (Desserialização e Serialização?)
public class ItemList
{
    public List<Item> Values;

    public ItemList()
    {
        Values = [];
    }

    public override string ToString()
    {
        string list = string.Empty;

        foreach (var item in Values)
        {
            list += item.ToString();
        }

        return list;
    }

    public void SequentializeIDs()
    {
        if (Values.Count <= 1) return;

        List<Item> temporaryList = Values;
        temporaryList[0].Id.Value = 1;

        for (int i = 0; i < temporaryList.Count - 1; i++)
        {
            temporaryList[i + 1].Id.Value = temporaryList[i + 1].Id.Value - temporaryList[i].Id.Value > 1 ? temporaryList[i].Id.Value + 1 : temporaryList[i + 1].Id.Value;
        }

        Values = temporaryList;
    }

    public Item? GetItemInList(string itemName)
    {
        return Values.FirstOrDefault(
            item => item.Nome.Value.Equals(itemName, StringComparison.CurrentCultureIgnoreCase));
    }
}