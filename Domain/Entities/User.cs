using Core.Persistence.Repositories;

namespace Domain.Entities;

public class User : Entity<Guid>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Platform { get; set; }

    public User()
    {
        
    }

    public User(Guid id, string name, string email, string platform)
    {
        Id = id;
        Name = name;
        Email = email;
        Platform = platform;
    }
}
