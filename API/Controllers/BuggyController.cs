using API.Controllers;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API;

public class BuggyController : BaseApiController
{
    private readonly DataContext _context;
    public BuggyController(DataContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetSecret()
    {
        return "secret text";
    }

    
    [HttpGet("not_found")]
    // This method is used to get a user by ID from the database.
    // If the user does not exist, it returns a NotFound response.
    public ActionResult<AppUser> GetUser()
    {
        // Trying to find a user with ID -1, which does not exist.
        var user = _context.Users.Find(-1);
        
        // If the user is not found, return a NotFound response.
        if(user == null) 
        {
            return NotFound();
        }

        return user;
    }
    [HttpGet("server-error")]
    public ActionResult<string> GetServerError()
    {
        var thing = _context.Users.Find(-1);
        var thingToReturn = thing.ToString();

        return thingToReturn;

    }
    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest()
    {
        return BadRequest("This is a bad request");
    }


}
