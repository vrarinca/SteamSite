using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ImportData.Models;
using Microsoft.Data.SqlClient;

namespace SteamGames.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly string _connectionString; // Replace with your database connection string

        public GameController()
        {
            // Initialize your database connection string
            _connectionString = "Server = DESKTOP-5CT3M5O; Database= steamgames; Trusted_Connection= True; Integrated Security= True; TrustServerCertificate = True";
        }


        // GET api/game/getgames
        [HttpGet("getgames")]
        public IActionResult GetGames()
        {
            try
            {
                List<Game> games = new List<Game>();

                // Create a database connection
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Create a SQL command to retrieve games from the database
                    using (SqlCommand command = new SqlCommand("SELECT TOP 100 Appid, Name FROM Game", connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Map database columns to your Game class properties
                                Game game = new Game
                                {
                                    Appid = Convert.ToInt32(reader["Appid"]),
                                    Name = reader["Name"].ToString()
                                };

                                games.Add(game);
                            }
                        }
                    }
                }

                return Ok(games);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("getgamedetails/{appid}")]
        public IActionResult GetGameDetails(int appid)
        {
            try
            {
                GameDetails gameDetails = null;

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Create a SQL command to retrieve game details from the database
                    string sqlQuery = @"
                SELECT steam_appid, type, name, required_age, is_free, dlc, short_description, categories, genres
                FROM GameDetails
                WHERE steam_appid = @appid";

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@appid", appid);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                gameDetails = new GameDetails
                                {
                                    steam_appid = reader["steam_appid"].ToString(),
                                    type = reader["type"].ToString(),
                                    name = reader["name"].ToString(),
                                    required_age = reader["required_age"].ToString(),
                                    is_free = reader["is_free"].ToString(),
                                    dlc = reader["dlc"].ToString(),
                                    short_description = reader["short_description"].ToString(),
                                    categories = reader["categories"].ToString(),
                                    genres = reader["genres"].ToString()
                                };
                            }
                        }
                    }
                }

                if (gameDetails != null)
                {
                    return Ok(gameDetails);
                }
                else
                {
                    return NotFound("Game details not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
