using System.Net;
using System.Net.Http.Json;
using CarRental.IntegrationTests.Infrastructure;
using MiA_Lr_3.DTOs.Cars;
using MiA_Lr_3.Models.Documents;
using MiA_Lr_3.Models.Enum;
using MongoDB.Driver;

namespace CarRental.IntegrationTests.Controllers
{
    /// <summary>
    /// ЛР5, Завдання 4. Інтеграційні тести CarController:
    /// HttpClient → Controller → Service → Repository → MongoDB (Mongo2Go).
    /// </summary>
    public class CarControllerTests : BaseIntegrationTest
    {
        // Валідний ObjectId, якого точно немає в БД
        private const string NonExistingId = "6748a68293acd000228f0000";

        public CarControllerTests(CustomWebApplicationFactory factory) : base(factory) { }

        private static CreateCarDto ValidCreateDto() => new()
        {
            Brand = "Skoda",
            Model = "Octavia",
            RegistrationNumber = "BC5555AA",
            Class = CarClass.Business,
            DailyRate = 1800m,
            CurrentMileage = 0
        };

        private Task<long> CountCarsAsync() =>
            Cars.CountDocumentsAsync(FilterDefinition<CarDocument>.Empty);

        // ------------------------------------------------------------
        // GET: /api/car
        // ------------------------------------------------------------
        [Fact]
        public async Task GetCars_ReturnsSeededItems()
        {
            // Act
            var response = await Client.GetAsync("/api/car");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var cars = await response.Content.ReadFromJsonAsync<List<CarResponseDto>>();
            Assert.NotNull(cars);
            Assert.Equal(3, cars.Count);
            Assert.Contains(cars, c => c.Brand == "Toyota");
        }

        [Fact]
        public async Task GetCars_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange — клієнт без токена
            var anonymousClient = Factory.CreateClient();

            // Act
            var response = await anonymousClient.GetAsync("/api/car");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ------------------------------------------------------------
        // GET: /api/car/{id}
        // ------------------------------------------------------------
        [Fact]
        public async Task GetCarById_ReturnsCorrectItem()
        {
            // Act
            var response = await Client.GetAsync($"/api/car/{DatabaseSeeder.Car1Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var car = await response.Content.ReadFromJsonAsync<CarResponseDto>();
            Assert.NotNull(car);
            Assert.Equal(DatabaseSeeder.Car1Id, car.Id);
            Assert.Equal("AA1111AA", car.RegistrationNumber);
        }

        [Fact]
        public async Task GetCarById_ReturnsNotFound_ForNonExistingId()
        {
            // Act
            var response = await Client.GetAsync($"/api/car/{NonExistingId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // ------------------------------------------------------------
        // POST: /api/car
        // ------------------------------------------------------------
        [Fact]
        public async Task CreateCar_ReturnsCreated_AndSavesToDatabase()
        {
            // Arrange
            var dto = ValidCreateDto();

            // Act
            var response = await Client.PostAsJsonAsync("/api/car", dto);

            // Assert — HTTP-відповідь
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);
            var created = await response.Content.ReadFromJsonAsync<CarResponseDto>();
            Assert.NotNull(created);
            Assert.False(string.IsNullOrWhiteSpace(created.Id));

            // Assert — документ справді збережено в БД
            var saved = await Cars.Find(c => c.Id == created.Id).FirstOrDefaultAsync();
            Assert.NotNull(saved);
            Assert.Equal("Skoda", saved.Brand);
            Assert.Equal(4, await CountCarsAsync());   // 3 seeded + 1 новий
        }

        [Theory]
        [InlineData("bc5555aa", 1800)]   // номер не у форматі AA9999AA
        [InlineData("BC5555AA", 0)]      // денна ставка має бути > 0
        public async Task CreateCar_ReturnsBadRequest_ForInvalidData(string regNumber, int dailyRate)
        {
            // Arrange
            var dto = ValidCreateDto();
            dto.RegistrationNumber = regNumber;
            dto.DailyRate = dailyRate;

            // Act
            var response = await Client.PostAsJsonAsync("/api/car", dto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(3, await CountCarsAsync());   // нічого не додано
        }

        [Fact]
        public async Task CreateCar_ReturnsForbidden_ForUserRole()
        {
            // Arrange — клієнт зі звичайною роллю User
            var userClient = CreateClientWithRole("User");

            // Act
            var response = await userClient.PostAsJsonAsync("/api/car", ValidCreateDto());

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        // ------------------------------------------------------------
        // PUT: /api/car/{id}
        // ------------------------------------------------------------
        [Fact]
        public async Task UpdateCar_ReturnsNoContent_AndUpdatesInDatabase()
        {
            // Arrange
            var dto = new UpdateCarDto
            {
                RegistrationNumber = "AA1111AA",
                DailyRate = 1500m,
                CurrentMileage = 12000,
                IsAvailable = false
            };

            // Act
            var response = await Client.PutAsJsonAsync($"/api/car/{DatabaseSeeder.Car1Id}", dto);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            var fromDb = await Cars.Find(c => c.Id == DatabaseSeeder.Car1Id).FirstAsync();
            Assert.Equal(1500m, fromDb.DailyRate);
            Assert.Equal(12000, fromDb.CurrentMileage);
            Assert.False(fromDb.IsAvailable);
            Assert.Equal("Toyota", fromDb.Brand);   // поля поза DTO не змінились
        }

        [Fact]
        public async Task UpdateCar_ReturnsNotFound_ForNonExistingId()
        {
            // Arrange
            var dto = new UpdateCarDto { RegistrationNumber = "AA1111AA", DailyRate = 1500m, CurrentMileage = 0 };

            // Act
            var response = await Client.PutAsJsonAsync($"/api/car/{NonExistingId}", dto);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateCar_ReturnsBadRequest_ForNegativeMileage()
        {
            // Arrange
            var dto = new UpdateCarDto { RegistrationNumber = "AA1111AA", DailyRate = 1500m, CurrentMileage = -5 };

            // Act
            var response = await Client.PutAsJsonAsync($"/api/car/{DatabaseSeeder.Car1Id}", dto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var fromDb = await Cars.Find(c => c.Id == DatabaseSeeder.Car1Id).FirstAsync();
            Assert.Equal(10000, fromDb.CurrentMileage);   // у БД нічого не змінилось
        }

        // ------------------------------------------------------------
        // DELETE: /api/car/{id}
        // ------------------------------------------------------------
        [Fact]
        public async Task DeleteCar_RemovesItemFromDatabase()
        {
            // Act
            var response = await Client.DeleteAsync($"/api/car/{DatabaseSeeder.Car2Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.Equal(0, await Cars.CountDocumentsAsync(c => c.Id == DatabaseSeeder.Car2Id));
            Assert.Equal(2, await CountCarsAsync());   // 3 − 1
        }

        [Fact]
        public async Task DeleteCar_ReturnsNotFound_ForNonExistingId()
        {
            // Act
            var response = await Client.DeleteAsync($"/api/car/{NonExistingId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
