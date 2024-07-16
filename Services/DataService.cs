using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Npgsql;
using WpfAlbom.Models;
using System.Net.Http.Json;

namespace WpfAlbom.Services
{
    public class DataService
    {
        private readonly string _postgresConnectionString;
        private readonly string _mongoConnectionString;

        public DataService()
        {
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "./config.json");
            var config = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(configPath));
            _postgresConnectionString = config["PostgresConnectionString"];
            _mongoConnectionString = config["MongoConnectionString"];
        }


        //Метод получения данных
        public async Task<List<T>> GetAlbumsAndPhotoAsync<T>(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                List<T> items = await client.GetFromJsonAsync<List<T>>(url) ?? new List<T>();
                return items;
            }
        }



        //Метод Сохранение данных
        public async Task SaveAlbumsAndPhotosToDatabaseAsync(List<Album> albums, List<Photo> photos)
        {
            var mongoClient = new MongoClient(_mongoConnectionString);
            var database = mongoClient.GetDatabase("AlbumMongo");
            var albumCollection = database.GetCollection<Album>("albums");

            using var connection = new NpgsqlConnection(_postgresConnectionString);
            await connection.OpenAsync();

            //Удаление данных
            var deleteAlbumsCommand = new NpgsqlCommand("DELETE FROM albums", connection);
            await deleteAlbumsCommand.ExecuteNonQueryAsync();
            await albumCollection.DeleteManyAsync(new BsonDocument());

            foreach (var album in albums)
            {
                try
                {
                    var command = new NpgsqlCommand("INSERT INTO albums (user_id, id, title) VALUES (@user_id, @id, @title)", connection);
                    command.Parameters.AddWithValue("user_id", album.UserId);
                    command.Parameters.AddWithValue("id", album.Id);
                    command.Parameters.AddWithValue("title", album.Title ?? (object)DBNull.Value);

                    await command.ExecuteNonQueryAsync();

                    album._id = ObjectId.GenerateNewId().ToString();
                    album.Photos = photos.FindAll(photo => photo.AlbumId == album.Id);

                    await albumCollection.InsertOneAsync(album);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при сохранении альбома: {ex.Message}");
                }
            }
        }
    }
}
