using Application.Features.Users.Dtos;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Users.Commands;

public class CreateUserCommand : IRequest<CreatedUserResponseDto>
{
    public string Name { get; set; }
    public string Email { get; set; }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreatedUserResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<CreatedUserResponseDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            User user = new()
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email
            };
            await _userRepository.AddAsync(user);

            return _mapper.Map<CreatedUserResponseDto>(user);
        }
    }
}
