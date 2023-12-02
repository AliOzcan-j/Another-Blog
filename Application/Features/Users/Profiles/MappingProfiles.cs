using Application.Features.Users.Commands;
using Application.Features.Users.Dtos;
using AutoMapper;
using Core.Application.Responses;
using Core.Persistence.Paging;
using Domain.Entities;

namespace Application.Features.Users.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<User, CreateUserCommand>().ReverseMap();
        CreateMap<User, CreatedUserResponseDto>().ReverseMap();

        CreateMap<User, GetListUserListItemDto>().ReverseMap();
        CreateMap<Paginate<User>, GetListResponse<GetListUserListItemDto>>().ReverseMap();
    }
}
