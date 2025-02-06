using TelegramBot.Data;
using TelegramBot.Infrastructure;

namespace TelegramBot.Domain;

public class ItemSearchResult
{
    private string FinalResult { get; set; } = string.Empty;
    public SearchStatus Status { get; set; }
    private Dictionary<int, SearchStatus> Scenario;
    private List<Item> Result { get; set; } = new();

    public ItemSearchResult()
    {
        Scenario = new Dictionary<int, SearchStatus>{
            {0, SearchStatus.NotFound},
            {1, SearchStatus.Found},
            {2, SearchStatus.MoreThanOne}
        };
    }

    private SearchStatus GetSearchStatus(int quantityOfItens)
    {
        return Scenario[quantityOfItens];
    }

    public ItemSearchResult GetItemSearchResult(List<Item> result)
    {
        Result = result;
        var searchStatus = GetSearchStatus(Result.Count);
        Verify(searchStatus);

        return new ItemSearchResult()
        {
            FinalResult = FinalResult,
            Status = Status
        };
    }

    private void Verify(SearchStatus searchStatus)
    {
        Dictionary<SearchStatus, Action> scenario = new()
        {
            {SearchStatus.NotFound, ItemNotFound},
            {SearchStatus.Found, ItemFound},
            {SearchStatus.MoreThanOne, MoreThanOneItem}
        };

        if(scenario.TryGetValue(searchStatus, out Action? action))
        {
            action.Invoke();
        }
    }

    private void ItemNotFound()
    {
        FinalResult = "Item não encontrado.";
        Status = SearchStatus.NotFound;
    }

    private void ItemFound()
    {
        FinalResult = "Item encontrado";
        Status = SearchStatus.Found;
    }

    private void MoreThanOneItem()
    {
        foreach (var item in Result)
            {
                var precoFormatado = item.Preco.ToString("C");
                FinalResult += $"{item.Nome} - {item.Marca} - {precoFormatado}\n";
            }
        
        Status = SearchStatus.MoreThanOne;
    }
}