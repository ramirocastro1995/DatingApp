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
        // Check if the username already exists in the database
        if(await UserExists(registerDto.Username)) 
            return BadRequest("Username is taken");

        // Initialize HMACSHA512 to generate a random key
        using var hmac = new HMACSHA512(); 

        // Create a new user with the input username and password
        var user = new AppUser
        {
            UserName = registerDto.Username.ToLower(),
            // Convert the password into a hash using the generated key
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            // Store the generated key as the password salt
            PasswordSalt = hmac.Key
        };
        
        // Add the new user to the database
        _context.Users.Add(user);
        // Save the changes to the database
        await _context.SaveChangesAsync();
    
        // Return the new user's details with a generated token
        return new UserDto
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user)
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        // Find the user in the database using the username provided in the loginDto
        var user = await _context.Users.FirstOrDefaultAsync(x=> x.UserName == loginDto.Username);

        // If the user does not exist, return an Unauthorized response
        if(user == null) return Unauthorized();
    
        // Create an instance of HMACSHA512 with the user's password salt to hash the password provided in the loginDto
        using var hmac = new HMACSHA512(user.PasswordSalt);
    
        // Compute the hash of the password provided in the loginDto
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
    
        // Compare the computed hash with the user's password hash
        for(int i = 0 ; i< computedHash.Length ;i++)
        {
            // If the hashes do not match, return an Unauthorized response with a message indicating that the password is invalid
            if(computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
        }
    
        // If the hashes match, return the user's information and a token
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
