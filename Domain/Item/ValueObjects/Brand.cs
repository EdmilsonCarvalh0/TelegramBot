namespace Domain.Item;

public class Brand
{
    private string _value;
    public string Value { 
        get => _value; 
        set
        {
            if(string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Informe a marca por favor.");

            _value = char.ToUpper(value[0]) + value[1..].ToLower();
        }
    }
    public static Brand Default => new ("Marca desconhecida");

    public Brand(string value)
    {
        _value = value;
    }

    public override string ToString() => Value;
}