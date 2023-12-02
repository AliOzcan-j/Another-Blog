using Application.Features.Users.Commands;
using Application.Features.Users.Dtos;
using Application.Features.Users.Queries.GetList;
using Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateUserCommand createUserCommand)
        {
            CreatedUserResponseDto response = await Mediator.Send(createUserCommand);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            GetListUserQuery getListUserQuery = new();
            GetListResponse<GetListUserListItemDto> response = await Mediator.Send(getListUserQuery);
            return Ok(response);
        }
    }
}
