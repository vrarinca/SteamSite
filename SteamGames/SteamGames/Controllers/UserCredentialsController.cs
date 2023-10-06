using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SteamGames.Models;
using ImportData;

namespace SteamGames.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserCredentialsController : ControllerBase
    {
        private readonly UserCredentialsContext _dbcontext;

        public UserCredentialsController(UserCredentialsContext context)
        {
            _dbcontext = context;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(UserCredentials credentials)
        {
            // Find the user in the database by email
            var user = await _dbcontext.UserCredentials.FirstOrDefaultAsync(u => u.email == credentials.email);

            if (user == null)
            {
                // User not found
                return Unauthorized(new { message = "Invalid credentials" });
            }

            // Check if the password matches (you should use a secure password hashing library)
            if (VerifyPasswordHash(credentials.password, user.password))
            {
                // Successful login
                return Ok(new { message = "Login successful" });
            }
            else
            {
                // Invalid password
                return Unauthorized(new { message = "Invalid credentials" });
            }
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            // Implement password verification logic here.
            // You should use a secure password hashing library like BCrypt.
            // For simplicity, we assume a basic string comparison here.
            return password == storedHash;
        }
    }
}