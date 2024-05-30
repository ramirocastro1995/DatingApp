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
    private readonly IPhotoService _photoService;
    public UsersController(IUserRepository userRepository, IMapper mapper,
    IPhotoService photoService)
    {
        _mapper = mapper;
        _userRepository = userRepository;
        _photoService = photoService;
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
        // Retrieve the user information using the username
        var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

        // If the user does not exist, return a NotFound result
        if (user == null) return NotFound();

        // Map the updated user information from the DTO to the user entity
        _mapper.Map(memberUpdateDto, user);

        // Save the updated user information to the database
        if (await _userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

        if(user == null) return NotFound();
        var result = await _photoService.AddPhotoAsync(file);
        if(result.Error != null) return BadRequest(result.Error.Message);

        var photo = new Photo{
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if(user.Photos.Count == 0) photo.IsMain = true;
        user.Photos.Add(photo);
        if(await _userRepository.SaveAllAsync()) return _mapper.Map<PhotoDto>(photo);

        return BadRequest("Problem adding photo");
    }
}

