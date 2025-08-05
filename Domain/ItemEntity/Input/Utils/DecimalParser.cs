using System.Globalization;

namespace TelegramBot.Domain.Item.Input.Utils;

public static class DecimalParser
{
    public static decimal ParseFlexible(string input)
    {
        bool sucess;

        if (input.Contains(',') && !input.Contains('.'))
        {
            sucess = decimal.TryParse(input, NumberStyles.Any, new CultureInfo("pt-BR"), out var resultPtBr);
            if (sucess) return resultPtBr;
        }

        sucess = decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var resultInvariant);
        if (sucess) return resultInvariant;
        
        throw new FormatException($"Valor decimal inválido: '{input}'. Use ponto ou vígula como separador decimal.");
    }
}