using System.Globalization;

namespace Domain.Item;

public class Price
{
    private decimal _value;
    public decimal Value { 
        get => _value;
        set
        {
            if(value < 0) throw new ArgumentException("O preço não pode ser negativo.");

            _value = value;
        }
    }
    public static Price Default => new(0);

    public Price(decimal value)
    {
        _value = value;
    }

    public override string ToString() =>
        Value.ToString("C", new CultureInfo("pt-BR"));
}