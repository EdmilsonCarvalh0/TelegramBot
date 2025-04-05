namespace Domain.Item;

public class Name
{
    private string _value;
    public string Value { 
        get => _value; 
        set
        {
            if(string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Informe o nome, por favor.");

            _value = char.ToUpper(value[0]) + value[1..].ToLower();
        } 
    }
    public static Name Default => new ("Nome desconhecido");

    public Name(string value)
    {        
        _value = value;
    }

    public override string ToString() => Value;
}