﻿using System.Linq;
using System.Threading.Tasks;
using Chat.API.Application.Commands;
using Chat.API.Application.Queries;
using Chat.API.Dtos;
using Chat.API.Models;
using Chat.API.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Chat.API.Controllers
{
    [Route("api/users")]
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly IMediator _mediator;
        private readonly IUserQueries _userQueries;

        public UsersController(
            IIdentityService identityService,
            IMediator mediator,
            IUserQueries userQueries)
        {
            _identityService = identityService;
            _mediator = mediator;
            _userQueries = userQueries;
        }

        [HttpGet("{id}", Name = nameof(GetUserById))]
        public async Task<IActionResult> GetUserById([FromRoute] string id)
        {
            var myId = _identityService.GetUserIdentity();
            var myPermissions = _identityService.GetPermissions();

            if (myId != "system" && myId != id && !myPermissions.Contains("users.get_others"))
            {
                return Unauthorized();
            }

            var user = await _userQueries.GetUserByIdAsync(id);
            return Ok(user);
        }

        [HttpPatch("{id}", Name = nameof(UpdateUserById))]
        public async Task<IActionResult> UpdateUserById([FromRoute] string id, [FromBody] JsonPatchDocument<UserDto> patch)
        {
            var testDto = new UserDto()
            {
                Profile = new ProfileDto()
            };
            patch.ApplyTo(testDto, ModelState);
            TryValidateModel(testDto);
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(ModelState));
            }

            var myId = _identityService.GetUserIdentity();
            var myPermissions = _identityService.GetPermissions();
            if (myId != "system" && myId != id && !myPermissions.Contains("users.update_others"))
            {
                return Unauthorized();
            }

            await _mediator.Publish(new UpdateUserCommand(id, patch));
            return NoContent();
        }
    }
}
