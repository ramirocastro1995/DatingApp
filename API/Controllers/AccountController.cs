using System.Security.Cryptography;
using System.Text;
using API.Controllers;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API;

public class AccountController : BaseApiController
{
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    //Dependency Injection
    public AccountController(DataContext context,ITokenService tokenService)
    {
        _tokenService = tokenService;
        _context = context;
    }
    /// <summary>
    /// Register a user and hash the password
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    [HttpPost("register")]//POST : api/account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        //Check if user exist by name
        if(await UserExists(registerDto.Username)) return BadRequest("Username is taken");

        // Generate Random key as password salt
        using var hmac = new HMACSHA512(); 

        var user = new AppUser
        {
            UserName = registerDto.Username.ToLower(),
            //Generate hash for password
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };
        //add the user and wait por the save.AWAIT is expected
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return new UserDto
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user)
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        //Finding if user exists
        var user = await _context.Users.FirstOrDefaultAsync(x=> x.UserName == loginDto.Username);

        if(user == null) return Unauthorized();
        //Checking if Passwords are the same
        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computerHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        for(int i = 0 ; i< computerHash.Length ;i++)
        {
            //If i is different in comparasion,fail
            if(computerHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
        }
        return new UserDto
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user)
        };

    }

    private async Task<bool> UserExists(string username)
    {
        return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
    }

}
