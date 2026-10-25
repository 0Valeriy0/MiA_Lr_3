using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace MiA_Lr_3.Data
{
    public class MongoDbClient
    {
        private static IMongoDatabase _db = null!;
        private static MongoDbClient? _instance;

        public static MongoDbClient Instance => _instance ??= new MongoDbClient();

        private MongoDbClient()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Зчитуємо тільки з файлу appsettings.json.
            var connectionString = config["MongoDbSettings:ConnectionString"]
                ?? throw new InvalidOperationException("Рядок підключення 'MongoDbSettings:ConnectionString' не знайдено в appsettings.json");

            var databaseName = config["MongoDbSettings:DatabaseName"] ?? "CarRentalDb";

            var client = new MongoClient(connectionString);
            _db = client.GetDatabase(databaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string name) => _db.GetCollection<T>(name);
    }
}