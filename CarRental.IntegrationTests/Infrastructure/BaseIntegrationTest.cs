using System.Net.Http.Headers;
using MiA_Lr_3.Models.Documents;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace CarRental.IntegrationTests.Infrastructure
{

    /// Базовий клас інтеграційних тестів: HttpClient з токеном Admin
    /// та заповнення БД початковими даними перед кожним тестом.

    public class BaseIntegrationTest : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        protected readonly CustomWebApplicationFactory Factory;
        protected readonly IServiceProvider Services;
        protected readonly HttpClient Client;   // клієнт з роллю Admin

        public BaseIntegrationTest(CustomWebApplicationFactory factory)
        {
            Factory = factory;
            Services = factory.Services;
            Client = CreateClientWithRole("Admin");
        }

        // HttpClient із JWT-токеном потрібної ролі
        protected HttpClient CreateClientWithRole(string role)
        {
            var client = Factory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Factory.GenerateToken(role));
            return client;
        }

        // Прямий доступ до колекції — щоб перевірити, що дані справді змінились у БД
        protected IMongoCollection<CarDocument> Cars =>
            Services.GetRequiredService<IMongoDatabase>().GetCollection<CarDocument>("cars");

        // xUnit викликає це ПЕРЕД КОЖНИМ тестом → ізоляція тестів
        public async Task InitializeAsync()
        {
            using var scope = Services.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
            await seeder.SeedCarsAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
