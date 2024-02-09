using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;

namespace API;

public class TokenService : ITokenService
{
    // Private read-only field for symmetric security key
    private readonly SymmetricSecurityKey _key;
    
    public TokenService(IConfiguration config)
    {
        // Initialize the symmetric security key with the token key from the configuration
        // The token key is converted from a string to a byte array using UTF8 encoding
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));   
    }
    public string CreateToken(AppUser user)
    {
        // Create a list of claims. In this case, we are adding only one claim: the username of the user
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
        };
        
        // Create signing credentials using a symmetric security key and a security algorithm (HMACSHA512 in this case)
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
        
        // Create a security token descriptor which will be used to create the JWT token
        // This descriptor includes the subject (which is a ClaimsIdentity object containing our list of claims), 
        // the expiration date of the token (which is set to 7 days from now), 
        // and the signing credentials we created earlier
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = creds
        };
        
        // Create a token handler which will be used to create and write the token
        var tokenHandler = new JwtSecurityTokenHandler();
        
        // Use the token handler to create a token using the token descriptor
        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        // Write the token and return it
        return tokenHandler.WriteToken(token);
    }
}
