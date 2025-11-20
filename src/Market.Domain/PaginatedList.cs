namespace Market.Domain;

public class PaginatedList<T>(List<T> items, int totalCount)
{
    public List<T> Items { get; set; } = items;
    public int TotalCount { get; set; } = totalCount;
}