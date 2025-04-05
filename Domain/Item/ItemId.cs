namespace Domain.Item;

public class ItemId
{
    private int _value;
    public int Value { 
        get => _value;
        set
        {
            if(value <= 0) throw new ArgumentException("ID deve ser positivo.");

            _value = value;
        }    
    }
    public static ItemId Default => new(1);

    public ItemId(int value)
    {
        _value = value;
    }

    public override string ToString() => Value.ToString();
}