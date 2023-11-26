namespace Core.Persistence.Repositories;

public class Audit<TId>
{
    public DateTime CreatedDate { get; set; }
    public TId CreatedUserId { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public TId? UpdatedUserId { get; set; }
    public DateTime? DeletedDate { get; set; }
    public TId? DeletedUserId { get; set; }

    public Audit()
    {
        CreatedDate = default;
        CreatedUserId = default;
    }

    public Audit(TId createdUserId, TId? updatedUserId, TId? deletedUserId)
    {
        CreatedUserId = createdUserId;
        UpdatedUserId = updatedUserId;
        DeletedUserId = deletedUserId;
    }
}
