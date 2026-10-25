using MongoDB.Driver;

namespace MiA_Lr_3.Data;

public class MongoDbClient
{
    private static IMongoDatabase _db = null!;
    private static MongoDbClient? _instance;

    public static MongoDbClient Instance
    {
        get => _instance ??= new MongoDbClient();
    }
    private MongoDbClient()
    {
        // Рядок підключення до локального сервера MongoDB
        var connectionString = "mongodb+srv://gavronskiyvaleriy2008_db_user:5H11JeWbDIWeYgNx@myclaster.zxkv64t.mongodb.net";
        var client = new MongoClient(connectionString);

        // Назва бази даних для проекту
        _db = client.GetDatabase("CarRentalDb");
    }
    // Метод отримання потрібної колекції (таблиці)
    public IMongoCollection<T> GetCollection<T>(string name) => _db.GetCollection<T>(name);
}
