using TelegramBot.Data;
using TelegramBot.Infrastructure;

namespace TelegramBot.Service;

public class SearchResultHandler
{
    private Dictionary<int, SearchStatus> Scenario;
    private List<Item> PrimaryResult { get; set; } = new();
    private string FinalResult { get; set; } = string.Empty;
    private SearchStatus FinalStatus { get; set; }

    public SearchResultHandler()
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

    public SearchResultDTO GetItemSearchResult(List<Item> result)
    {
        PrimaryResult = result;
        var searchStatus = GetSearchStatus(PrimaryResult.Count);
        Verify(searchStatus);

        return new SearchResultDTO()
        {
            Result = FinalResult,
            Status = FinalStatus
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
        FinalStatus = SearchStatus.NotFound;
    }

    private void ItemFound()
    {
        FinalResult = "Item encontrado";
        FinalStatus = SearchStatus.Found;
    }

    private void MoreThanOneItem()
    {
        foreach (var item in PrimaryResult)
            {
                var precoFormatado = item.Preco.ToString("C");
                FinalResult += $"{item.Nome} - {item.Marca} - {precoFormatado}\n";
            }
        
        FinalStatus = SearchStatus.MoreThanOne;
    }
}