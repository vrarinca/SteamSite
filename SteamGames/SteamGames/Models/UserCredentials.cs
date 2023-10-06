using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SteamGames.Models
{
    public class UserCredentials
    {
        [Key]
        public string email { get; set; } = "";

        [Column(TypeName ="nvarchar(25)")]
        public string password { get; set; } = "";
        
        [Column(TypeName = "nvarchar(15)")]
        public string first_name { get; set; } = "";
        
        [Column(TypeName = "nvarchar(15)")]
        public string last_name { get; set; } = "";
    }
}
