namespace Core.Persistence.Paging;

public class Paginate<T> : BasePageableModel
{
    public IList<T> Items { get; set; }

    public Paginate()
    {
        Items = Array.Empty<T>();
    }
}
