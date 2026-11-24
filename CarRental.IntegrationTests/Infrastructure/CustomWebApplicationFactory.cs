using MiA_Lr_3.Models;
using MiA_Lr_3.Services.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mongo2Go;
using MongoDB.Driver;

namespace CarRental.IntegrationTests.Infrastructure
{

    /// Запускає застосунок у пам'яті та замінює підключення до MongoDB Atlas
    /// на тимчасовий локальний сервер Mongo2Go.

    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private MongoDbRunner? _runner;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // ConfigureTestServices виконується ПІСЛЯ реєстрацій з Program.cs,
            // тому тестові реєстрації гарантовано замінять робочі
            builder.ConfigureTestServices(services =>
            {
                // 1. Запускаємо тимчасовий локальний MongoDB
                var runner = MongoDbRunner.Start();
                _runner = runner;

                // 2. Видаляємо реєстрації, що вели в Atlas
                services.RemoveAll<IMongoClient>();
                services.RemoveAll<IMongoDatabase>();

                // 3. Реєструємо тестовий клієнт і тестову БД
                services.AddSingleton<IMongoClient>(_ => new MongoClient(runner.ConnectionString));
                services.AddSingleton<IMongoDatabase>(sp =>
                    sp.GetRequiredService<IMongoClient>().GetDatabase("IntegrationTestsDb"));

                // 4. Реєструємо seeder
                services.AddSingleton<DatabaseSeeder>();
            });
        }

        // Генерує справжній JWT через TokenService застосунку
        // (контролер має [Authorize], тому без токена буде 401)
        public string GenerateToken(string role)
        {
            using var scope = Services.CreateScope();
            var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
            return tokenService.GenerateAccessToken(new User
            {
                Id = "integration-test-user",
                Email = "test@test.com",
                Role = role
            });
        }

        // 5. Після тестів зупиняємо й видаляємо тимчасовий MongoDB
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _runner?.Dispose();
        }
    }
}
