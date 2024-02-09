using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Data;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API;

public class Seed
{
    // This method is used to seed users data into the database
    public static async Task SeedUsers(DataContext context)
    {
        // Check if there are any users in the database, if there are then return
        if(await context.Users.AnyAsync()) return;

        // Read the user data from the JSON file
        var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

        // Set the JSON serializer options to ignore case sensitivity
        var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};

        // Deserialize the user data from the JSON file into a list of AppUser objects
        var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

        // Iterate through each user in the list
        foreach(var user in users)
        {
            // Create a new HMACSHA512 instance for hashing the password
            using var hmac = new HMACSHA512();

            // Convert the username to lowercase
            user.UserName = user.UserName.ToLower();
    
            // Compute the hash of the password and assign it to the PasswordHash property of the user
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pas$$w@rd"));
    
            // Assign the key of the HMACSHA512 instance to the PasswordSalt property of the user
            user.PasswordSalt = hmac.Key;

            // Add the user to the Users DbSet
            context.Users.Add(user);
        }

        // Save the changes to the database
        await context.SaveChangesAsync();
    
    }

}
