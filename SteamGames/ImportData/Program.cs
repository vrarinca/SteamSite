using ImportData.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using Newtonsoft.Json.Linq;


namespace ImportData
{
    internal class Program
    {
        static void DisplayAllGames(List<Game> games)
        {
            foreach (Game game in games)
            {
                Console.WriteLine($"Appid: {game.Appid}, Name: {game.Name}");
            }
        }

        static void AddGameDetailsDb(GameDetails gameDetails, SqlConnection connection)
        {
            using (SqlCommand command = new SqlCommand("InsertGameDetails", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                DbParameter param_type = command.CreateParameter();
                param_type.ParameterName = "@type";
                param_type.Value = gameDetails.type;
                command.Parameters.Add(param_type);


                DbParameter param_name = command.CreateParameter();
                param_name.ParameterName = "@name";
                param_name.Value = gameDetails.name;
                command.Parameters.Add(param_name);

                DbParameter param_steam_appid = command.CreateParameter();
                param_steam_appid.ParameterName = "@steam_appid";
                param_steam_appid.Value = gameDetails.steam_appid;
                command.Parameters.Add(param_steam_appid);

                DbParameter param_required_age = command.CreateParameter();
                param_required_age.ParameterName = "@required_age";
                param_required_age.Value = gameDetails.required_age;
                command.Parameters.Add(param_required_age);

                DbParameter param_is_free = command.CreateParameter();
                param_is_free.ParameterName = "@is_free";
                param_is_free.Value = gameDetails.is_free;
                command.Parameters.Add(param_is_free);

                DbParameter param_dlc = command.CreateParameter();
                param_dlc.ParameterName = "@dlc";
                param_dlc.Value = gameDetails.dlc;
                command.Parameters.Add(param_dlc);

                DbParameter param_short_description = command.CreateParameter();
                param_short_description.ParameterName = "@short_description";
                param_short_description.Value = gameDetails.short_description;
                command.Parameters.Add(param_short_description);

                DbParameter param_categories = command.CreateParameter();
                param_categories.ParameterName = "@categories";
                param_categories.Value = gameDetails.categories;
                command.Parameters.Add(param_categories);

                DbParameter param_genres = command.CreateParameter();
                param_genres.ParameterName = "@genres";
                param_genres.Value = gameDetails.genres;
                command.Parameters.Add(param_genres);

                command.Connection = connection;

                command.ExecuteNonQuery();
            }
        }

        static async void AddGamesDetailsDb(string url, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonData = await response.Content.ReadAsStringAsync();

                        Dictionary<string, Dictionary<string, List<Game>>> deserializedData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<Game>>>>(jsonData);

                        List<Game> games = deserializedData["applist"]["apps"];

                        // try catch for logs 
                        foreach (Game game in games)
                        {
                            string url_game_details = "https://store.steampowered.com/api/appdetails?appids=" + game.Appid;

                            HttpResponseMessage response_game_details = await client.GetAsync(url_game_details);

                            if (response.IsSuccessStatusCode)
                            {
                                GameDetails gameDetails = new GameDetails();

                                string jsonDataGameDetails = await response_game_details.Content.ReadAsStringAsync();

                                JObject jsonObj = JObject.Parse(jsonDataGameDetails);

                                foreach (var property in jsonObj.Properties())
                                {
                                    // Add the steam_appid

                                    gameDetails.steam_appid = (string)property.Name;

                                    Console.WriteLine(gameDetails.steam_appid);

                                    foreach (var innerProperty in property.Value)
                                    {
                                        if (((JProperty)innerProperty).Name == "data")
                                        {
                                            JObject dataObject = (JObject)((JProperty)innerProperty).Value;

                                            // Add the game details

                                            // Add type
                                            if (string.IsNullOrEmpty((string)dataObject["type"]))
                                            {
                                                gameDetails.type = "Not provided";
                                            }
                                            else
                                            {
                                                gameDetails.type = (string)dataObject["type"];
                                            }

                                            // Add name 
                                            if (string.IsNullOrEmpty((string)dataObject["name"]))
                                            {
                                                gameDetails.name = "Not provided";
                                            }
                                            else
                                            {
                                                gameDetails.name = (string)dataObject["name"];
                                            }

                                            // Add required age
                                            if (string.IsNullOrEmpty((string)dataObject["required_age"]))
                                            {
                                                gameDetails.required_age = "Not provided";
                                            }
                                            else
                                            {
                                                gameDetails.required_age = (string)dataObject["required_age"];
                                            }

                                            // Add is free
                                            if (string.IsNullOrEmpty((string)dataObject["is_free"]))
                                            {
                                                gameDetails.is_free = "Not provided";
                                            }
                                            else
                                            {
                                                gameDetails.is_free = (string)dataObject["is_free"];
                                            }

                                            // Add short description
                                            if (string.IsNullOrEmpty((string)dataObject["short_description"]))
                                            {
                                                gameDetails.short_description = "Not provided";
                                            }
                                            else
                                            {
                                                gameDetails.short_description = (string)dataObject["short_description"];
                                            }


                                            if (dataObject.TryGetValue("dlc", out JToken dlcToken) && dlcToken.Type == JTokenType.Array)
                                            {
                                                JArray dlcArray = (JArray)dlcToken;

                                                string myString = "";

                                                foreach (var dlc in dlcArray)
                                                {
                                                    myString += dlc.ToString();
                                                    myString += ", ";
                                                }

                                                if (myString.Length >= 2)
                                                {
                                                    string modifiedString = myString.Substring(0, myString.Length - 2);
                                                    gameDetails.dlc = modifiedString;
                                                }
                                            }
                                            else
                                            {
                                                gameDetails.dlc = "Not provided";
                                            }

                                            if (dataObject.TryGetValue("categories", out JToken categoriesToken) && categoriesToken.Type == JTokenType.Array)
                                            {
                                                JArray categoriesArray = (JArray)categoriesToken;

                                                string myString = "";

                                                foreach (var category in categoriesArray)
                                                {
                                                    myString += category["description"].ToString();
                                                    myString += ", ";
                                                }

                                                if (myString.Length >= 2)
                                                {
                                                    string modifiedString = myString.Substring(0, myString.Length - 2);
                                                    gameDetails.categories = modifiedString;
                                                }
                                            }
                                            else
                                            {
                                                gameDetails.categories = "Not provided";
                                            }

                                            if (dataObject.TryGetValue("genres", out JToken genresToken) && genresToken.Type == JTokenType.Array)
                                            {
                                                JArray genresArray = (JArray)genresToken;

                                                string myString = "";

                                                foreach (var genre in genresArray)
                                                {
                                                    myString += genre["description"].ToString();
                                                    myString += ", ";
                                                }

                                                if (myString.Length >= 2)
                                                {
                                                    string modifiedString = myString.Substring(0, myString.Length - 2);
                                                    gameDetails.genres = modifiedString;
                                                }
                                            }
                                            else
                                            {
                                                gameDetails.genres = "Not provided";
                                            }

                                            //using (StreamWriter writer = new StreamWriter(filePath, true))
                                            //{
                                            //    writer.WriteLine($"Appid: {gameDetails.steam_appid}");
                                            //    writer.WriteLine($"Type: {gameDetails.type}");
                                            //    writer.WriteLine($"Name: {gameDetails.name}");
                                            //    writer.WriteLine($"Required Age: {gameDetails.required_age}");
                                            //    writer.WriteLine($"Is Free: {gameDetails.is_free}");
                                            //    writer.WriteLine($"Dlc: {gameDetails.dlc}");
                                            //    writer.WriteLine($"Categories: {gameDetails.categories}");
                                            //    writer.WriteLine($"Genres: {gameDetails.genres}");
                                            //    writer.WriteLine();
                                            //    writer.WriteLine();
                                            //}

                                            AddGameDetailsDb(gameDetails, connection);
                                        }
                                    }
                                }

                                Console.WriteLine();
                                continue;
                            }
                        }
                    }

                    else
                    {
                        Console.WriteLine($"Failed to retrieve data. Status code: {response.StatusCode}");
                    }
                }

                connection.Close();
            }
        }

        static void AddGameDb(List<Game> games, SqlConnection connection)
        {
            foreach (Game game in games)
            {
                if (game.Name == "")
                    continue;
                else
                {
                    using (SqlCommand command = new SqlCommand("InsertGamesFromList", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        DbParameter paramFrom = command.CreateParameter();
                        paramFrom.ParameterName = "@Appid";
                        paramFrom.Value = game.Appid;
                        command.Parameters.Add(paramFrom);


                        DbParameter paramTo = command.CreateParameter();
                        paramTo.ParameterName = "@Name";
                        paramTo.Value = game.Name;
                        command.Parameters.Add(paramTo);

                        command.Connection = connection;

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        static async void AddGamesDb(string url, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonData = await response.Content.ReadAsStringAsync();

                        Dictionary<string, Dictionary<string, List<Game>>> deserializedData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<Game>>>>(jsonData);

                        List<Game> games = deserializedData["applist"]["apps"];

                        DisplayAllGames(games);

                        // Add try catch for logs
                        AddGameDb(games, connection);
                    }

                    else
                    {
                        Console.WriteLine($"Failed to retrieve data. Status code: {response.StatusCode}");
                    }
                }

                connection.Close();
            }
        }


        static void Main(string[] args)
        {
            string url = $"https://api.steampowered.com/ISteamApps/GetAppList/v2/";

            string connectionString = "Server= DESKTOP-5CT3M5O; Database= steamgames; Integrated Security= True; TrustServerCertificate= True";

            //AddGamesDb(url, connectionString);
            AddGamesDetailsDb(url, connectionString);

            Console.ReadLine();
        }
    }
}

