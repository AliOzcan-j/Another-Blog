namespace Core.Persistence.Paging;

public abstract class BasePageableModel
{
    public int Index { get; set; }
    public int Size{ get; set; }
    public int Count { get; set; }
    public int Pages { get; set; }
    public bool HasPrevious => Index > 0;
    public bool HasNext => Index + 1 < Pages;
}