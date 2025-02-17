using System.Globalization;
using Newtonsoft.Json;

namespace TelegramBot.Data;

public class Item : ItemDataModel
{

    public override string ToString()
    {
        return $"{Id}. {Nome} - {Marca} - {Preco:C}\n";
    }

    public void SetNome(string attribute)
    {
        Nome = attribute;
    }

    public void SetMarca(string attribute)
    {
        Marca = attribute;
    }

    public void SetPreco(string attribute)
    {
        Preco = Convert.ToDecimal(attribute);
    }

}