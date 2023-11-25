namespace Core.Persistence.Repositories;

public class Entity<TId> : Audit<TId>
{
    public TId Id { get; set; }

    public Entity()
    {
        Id = default;
    }

    public Entity(TId id)
    {
        Id = id;
    }
}
