using TelegramBot.Domain;
using Domain.Item;

namespace TelegramBot.Service;

public class SearchResultHandler
{
    private List<Item> PrimaryResult { get; set; } = [];
    private string FinalResult { get; set; } = string.Empty;
    private SearchStatus FinalStatus { get; set; }
    private Dictionary<SearchStatus, Action> Scenario =>
        new()
        {
            {SearchStatus.NotFound, ItemNotFound},
            {SearchStatus.Found, ItemFound},
            {SearchStatus.MoreThanOne, MoreThanOneItem}
        };

    private SearchStatus CheckPrimaryResultStatus(int quantityOfItens)
    {
        if (quantityOfItens == 0) return SearchStatus.NotFound;
        if (quantityOfItens == 1) return SearchStatus.Found;
        return SearchStatus.MoreThanOne;
    }

    public SearchResult GetSearchResult(List<Item> result)
    {
        PrimaryResult = result;
        var searchStatus = CheckPrimaryResultStatus(PrimaryResult.Count);
        Verify(searchStatus);

        return new SearchResult()
        {
            Result = FinalResult,
            Status = FinalStatus
        };
    }

    private void Verify(SearchStatus searchStatus)
    {
        if(Scenario.TryGetValue(searchStatus, out Action? action))
        {
            action.Invoke();
        }
    }

    private void ItemNotFound()
    {
        FinalResult = string.Empty;
        FinalStatus = SearchStatus.NotFound;
    }

    private void ItemFound()
    {
        FinalResult = "Item encontrado";
        FinalStatus = SearchStatus.Found;
    }

    private void MoreThanOneItem()
    {
        FinalResult = string.Empty;

        foreach (var item in PrimaryResult)
        {
            FinalResult += item.ToString();
        }
        
        FinalStatus = SearchStatus.MoreThanOne;
    }

    public Item? GetItemToUpdate(int referenceNumber)
    {
        return PrimaryResult.FirstOrDefault(item => item.Id.Value == referenceNumber)!;
    }
}