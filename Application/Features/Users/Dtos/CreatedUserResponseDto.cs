namespace Application.Features.Users.Dtos;

public class CreatedUserResponseDto
{
    public Guid Id { get; set; } 
    public required string Name { get; set; }
    public required string Email { get; set; }
}