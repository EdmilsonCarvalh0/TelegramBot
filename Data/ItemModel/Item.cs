using System.Globalization;
using Newtonsoft.Json;

namespace TelegramBot.Data;

public class Item : ItemDataModel
{

    public override string ToString()
    {
        return $"{Nome} - {Marca} - {Preco:C}\n";
    }
}