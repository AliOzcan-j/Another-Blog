using Application.Features.Users.Dtos;
using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Responses;
using Core.Persistence.Paging;
using Domain.Entities;
using MediatR;

namespace Application.Features.Users.Queries.GetList;

public class GetListUserQuery : IRequest<GetListResponse<GetListUserListItemDto>>
{
    public class GetListUserQueryHandler : IRequestHandler<GetListUserQuery, GetListResponse<GetListUserListItemDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetListUserQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListUserListItemDto>> Handle(GetListUserQuery request, CancellationToken cancellationToken)
        {
            Paginate<User> users = await _userRepository.GetListAsync
                                                        (
                                                            index: 0,
                                                            size: 5,
                                                            cancellationToken: cancellationToken,
                                                            withDeleted: true
                                                        );
            return _mapper.Map<GetListResponse<GetListUserListItemDto>>(users);
        }
    }
}
