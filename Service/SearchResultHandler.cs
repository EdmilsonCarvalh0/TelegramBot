using TelegramBot.Data;
using TelegramBot.Domain;

namespace TelegramBot.Service;

public class SearchResultHandler
{
    private Dictionary<int, SearchStatus> ScenarioStatus;
    private Dictionary<SearchStatus, Action> Scenario;
    private List<Item> PrimaryResult { get; set; } = new();
    private string FinalResult { get; set; } = string.Empty;
    private SearchStatus FinalStatus { get; set; }

    public SearchResultHandler()
    {
        ScenarioStatus = new Dictionary<int, SearchStatus>{
            {0, SearchStatus.NotFound},
            {1, SearchStatus.Found},
            {2, SearchStatus.MoreThanOne}
        };

        Scenario = new Dictionary<SearchStatus, Action>
        {
            {SearchStatus.NotFound, ItemNotFound},
            {SearchStatus.Found, ItemFound},
            {SearchStatus.MoreThanOne, MoreThanOneItem}
        };
    }

    private SearchStatus GetSearchStatus(int quantityOfItens)
    {
        return ScenarioStatus[quantityOfItens];
    }

    public SearchResult GetSearchResult(List<Item> result)
    {
        PrimaryResult = result;
        var searchStatus = GetSearchStatus(PrimaryResult.Count);
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
        FinalResult = "";
        FinalResult = "Item encontrado";
        FinalStatus = SearchStatus.Found;
    }

    private void MoreThanOneItem()
    {
        foreach (var item in PrimaryResult)
            {
                FinalResult += item.ToString();
            }
        
        FinalStatus = SearchStatus.MoreThanOne;
    }

    public Item? GetItemToUpdate(int referenceNumber)
    {
        return PrimaryResult.FirstOrDefault(item => item.Id == referenceNumber)!;
    }
}