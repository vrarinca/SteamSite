using Microsoft.EntityFrameworkCore;

namespace SteamGames.Models
{
    public class UserCredentialsContext : DbContext
    {
        public UserCredentialsContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<UserCredentials> UserCredentials { get; set; }
    }
}
