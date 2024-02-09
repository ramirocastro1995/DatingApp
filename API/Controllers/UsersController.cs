using System.Security.Claims;
using API.Data;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
        _mapper = mapper;
        _userRepository = userRepository;
    }

    // This is a HttpGet method that asynchronously gets all the users from the repository
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        // Call the GetMembersAsync method from the _userRepository to get all the users
        var users = await _userRepository.GetMembersAsync();

        // Return the users with an Ok (200) status
        return Ok(users);
    }


    [HttpGet("{username}")] // /api/users/2
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        return await _userRepository.GetMemberAsync(username);

    }

    //Used to UPDATE
    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        // Retrieve the username from the claim
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Retrieve the user information using the username
        var user = await _userRepository.GetUserByUsernameAsync(username);

        // If the user does not exist, return a NotFound result
        if (user == null) return NotFound();

        // Map the updated user information from the DTO to the user entity
        _mapper.Map(memberUpdateDto, user);

        // Save the updated user information to the database
        if (await _userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update user");
    }
}

