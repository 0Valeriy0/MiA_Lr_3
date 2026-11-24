using MiA_Lr_3.Models;
using MiA_Lr_3.Models.Enum;
using MiA_Lr_3.Repositories.Interface;
using MiA_Lr_3.Services;
using MiA_Lr_3.Services.Interface;
using Moq;

namespace CarRental_Test.Tests.Services
{
    /// ЛР5, Завдання 2. Unit-тести CarContractService.
    /// Усі залежності (репозиторій договорів, сервіси клієнтів та авто) замінені mock-об'єктами Moq,
    /// тому тести не звертаються до MongoDB.
   
    public class CarContractServiceTests
    {
        // Mock-об'єкти всіх залежностей сервісу
        private readonly Mock<ICarContractRepository> _contractRepoMock = new();
        private readonly Mock<IClientService> _clientServiceMock = new();
        private readonly Mock<ICarService> _carServiceMock = new();
        private readonly CarContractService _service;

        public CarContractServiceTests()
        {
            // Передаємо в сервіс підроблені об'єкти (.Object) замість справжніх
            _service = new CarContractService(
                _contractRepoMock.Object,
                _clientServiceMock.Object,
                _carServiceMock.Object);
        }

        // Допоміжні методи для тестових даних 

        private static Client CreateClient(int age = 30, int experience = 10, decimal balance = 50000m) => new()
        {
            Id = "client1",
            FullName = "Іван Петренко",
            BirthDate = DateTime.Today.AddYears(-age),
            DrivingExperienceYears = experience,
            LicenseNumber = "ABC123456",
            Balance = balance
        };

        private static Car CreateCar(CarClass carClass = CarClass.Economy, bool isAvailable = true) => new()
        {
            Id = "car1",
            Brand = "Toyota",
            Model = "Corolla",
            RegistrationNumber = "AA1234BB",
            Class = carClass,
            DailyRate = 1000m,
            CurrentMileage = 1000,
            IsAvailable = isAvailable
        };

        private static CarContract CreateContract(string id, decimal totalCost,
            ContractStatus status = ContractStatus.Closed, DateTime? actualEnd = null,
            decimal extraMileage = 0m, string clientId = "client1") => new()
        {
            Id = id,
            ClientId = clientId,
            CarId = "car1",
            TotalCost = totalCost,
            Status = status,
            ActualEndDate = actualEnd,
            ExtraMileageCharge = extraMileage
        };

      
        // CalculateRentalCost — бізнес-задача 2 (чиста функція)
        

        [Theory]
        [InlineData(1000, 1, InsuranceType.Basic, 1000)]       // мінімальний термін
        [InlineData(1000, 6, InsuranceType.Basic, 6000)]       // ще без знижки
        [InlineData(1000, 7, InsuranceType.Basic, 6300)]       // межа: знижка 10%
        [InlineData(1000, 29, InsuranceType.Basic, 26100)]     // ще 10%
        [InlineData(1000, 30, InsuranceType.Basic, 24000)]     // межа: знижка 20%
        [InlineData(1000, 3, InsuranceType.FullCover, 4200)]   // +400 грн/доба за повну страховку
        public void CalculateRentalCost_VariousTerms_ReturnsExpectedCost(
            int dailyRate, int days, InsuranceType insurance, int expected)
        {
            // Arrange
            var start = new DateTime(2026, 1, 1);
            var end = start.AddDays(days);

            // Act
            decimal cost = _service.CalculateRentalCost(dailyRate, start, end, insurance);

            // Assert
            Assert.Equal((decimal)expected, cost);
        }

        [Fact]
        public void CalculateRentalCost_ZeroDays_ThrowsArgumentException()
        {
            // Arrange
            var date = new DateTime(2026, 1, 1);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _service.CalculateRentalCost(1000m, date, date, InsuranceType.Basic));
        }

        // GetTotalRevenueAsync — Where + Sum

        [Fact]
        public async Task GetTotalRevenueAsync_MixedContracts_SumsOnlyClosedInPeriod()
        {
            // Arrange
            var from = new DateTime(2026, 11, 1);
            var to = new DateTime(2026, 11, 30);
            var contracts = new List<CarContract>
            {
                CreateContract("1", 3000m, ContractStatus.Closed, new DateTime(2026, 11, 5), extraMileage: 600m), // 3600
                CreateContract("2", 2000m, ContractStatus.Closed, new DateTime(2026, 11, 20)),                    // 2000
                CreateContract("3", 5000m, ContractStatus.Active),                                                // ще не закритий
                CreateContract("4", 9999m, ContractStatus.Closed, new DateTime(2026, 10, 15))                     // поза періодом
            };
            _contractRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(contracts);

            // Act
            decimal revenue = await _service.GetTotalRevenueAsync(from, to);

            // Assert
            Assert.Equal(5600m, revenue);
            _contractRepoMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetTotalRevenueAsync_EmptyCollection_ReturnsZero()
        {
            // Arrange
            _contractRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<CarContract>());

            // Act
            decimal revenue = await _service.GetTotalRevenueAsync(DateTime.Today.AddDays(-30), DateTime.Today);

            // Assert
            Assert.Equal(0m, revenue);
        }

        [Fact]
        public async Task GetTotalRevenueAsync_FromAfterTo_ThrowsArgumentException()
        {
            // Arrange
            var from = new DateTime(2026, 12, 1);
            var to = new DateTime(2026, 11, 1);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetTotalRevenueAsync(from, to));

            // До репозиторію навіть не звертались — валідація спрацювала раніше
            _contractRepoMock.Verify(r => r.GetAllAsync(), Times.Never);
        }

        // GetAverageRentalCostAsync — Average

        [Fact]
        public async Task GetAverageRentalCostAsync_ThreeContracts_ReturnsRoundedAverage()
        {
            // Arrange
            var contracts = new List<CarContract>
            {
                CreateContract("1", 1000m),
                CreateContract("2", 2000m),
                CreateContract("3", 4000m)
            };
            _contractRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(contracts);

            // Act
            decimal average = await _service.GetAverageRentalCostAsync();

            // Assert
            Assert.Equal(2333.33m, average);
            _contractRepoMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAverageRentalCostAsync_EmptyCollection_ReturnsZero()
        {
            // Arrange
            _contractRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<CarContract>());

            // Act
            decimal average = await _service.GetAverageRentalCostAsync();

            // Assert
            Assert.Equal(0m, average);
        }

        // GetMostExpensiveContractAsync — MaxBy

        [Fact]
        public async Task GetMostExpensiveContractAsync_SeveralContracts_ReturnsMax()
        {
            // Arrange
            var expensive = CreateContract("2", 9000m);
            var contracts = new List<CarContract> { CreateContract("1", 1000m), expensive, CreateContract("3", 3000m) };
            _contractRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(contracts);

            // Act
            var result = await _service.GetMostExpensiveContractAsync();

            // Assert
            Assert.Same(expensive, result);
        }

        [Fact]
        public async Task GetMostExpensiveContractAsync_EmptyCollection_ReturnsNull()
        {
            // Arrange
            _contractRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<CarContract>());

            // Act
            var result = await _service.GetMostExpensiveContractAsync();

            // Assert
            Assert.Null(result);
        }

        // CountActiveContractsByClientAsync — Count + валідація
    
        [Fact]
        public async Task CountActiveContractsByClientAsync_ClientHasContracts_ReturnsActiveCount()
        {
            // Arrange
            _clientServiceMock.Setup(s => s.GetByIdAsync("client1")).ReturnsAsync(CreateClient());
            var contracts = new List<CarContract>
            {
                CreateContract("1", 1000m, ContractStatus.Active),
                CreateContract("2", 1000m, ContractStatus.Active),
                CreateContract("3", 1000m, ContractStatus.Closed),                      // закритий
                CreateContract("4", 1000m, ContractStatus.Active, clientId: "client2")  // інший клієнт
            };
            _contractRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(contracts);

            // Act
            int count = await _service.CountActiveContractsByClientAsync("client1");

            // Assert
            Assert.Equal(2, count);
            _clientServiceMock.Verify(s => s.GetByIdAsync("client1"), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task CountActiveContractsByClientAsync_EmptyId_ThrowsArgumentException(string clientId)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.CountActiveContractsByClientAsync(clientId));
        }

        [Fact]
        public async Task CountActiveContractsByClientAsync_ClientNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange — GetByIdAsync не налаштований, тому mock поверне null

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.CountActiveContractsByClientAsync("unknown"));

            _contractRepoMock.Verify(r => r.GetAllAsync(), Times.Never);
        }

        // CreateRentalAsync — бізнес-задача 1 (оренда та перевірка цензу)

        [Fact]
        public async Task CreateRentalAsync_ValidData_CreatesContractAndBlocksDeposit()
        {
            // Arrange
            var client = CreateClient(balance: 15000m);
            var car = CreateCar(CarClass.Economy);
            _clientServiceMock.Setup(s => s.GetByIdAsync("client1")).ReturnsAsync(client);
            _carServiceMock.Setup(s => s.GetByIdAsync("car1")).ReturnsAsync(car);
            _contractRepoMock.Setup(r => r.CreateAsync(It.IsAny<CarContract>()))
                .ReturnsAsync((CarContract c) => { c.Id = "contract1"; return c; }); // імітуємо MongoDB, що присвоює Id

            // Act
            var result = await _service.CreateRentalAsync("client1", "car1",
                DateTime.Today.AddDays(3), InsuranceType.Basic);

            // Assert — бізнес-правила
            Assert.Equal("contract1", result.Id);
            Assert.Equal(3000m, result.TotalCost);        // 3 доби × 1000
            Assert.Equal(10000m, result.DepositAmount);   // застава для Economy
            Assert.Equal(ContractStatus.Active, result.Status);
            Assert.Equal(5000m, client.Balance);          // 15000 − 10000
            Assert.False(car.IsAvailable);

            // Assert — виклики залежностей
            _contractRepoMock.Verify(r => r.CreateAsync(It.IsAny<CarContract>()), Times.Once);
            _clientServiceMock.Verify(s => s.UpdateAsync("client1", It.Is<Client>(c => c.Balance == 5000m)), Times.Once);
            _carServiceMock.Verify(s => s.UpdateAsync("car1", It.Is<Car>(c => !c.IsAvailable)), Times.Once);
        }

        [Fact]
        public async Task CreateRentalAsync_ClientNotFound_ThrowsArgumentException()
        {
            // Arrange — клієнта не налаштовано, mock поверне null
            _carServiceMock.Setup(s => s.GetByIdAsync("car1")).ReturnsAsync(CreateCar());

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.CreateRentalAsync("unknown", "car1", DateTime.Today.AddDays(3), InsuranceType.Basic));
        }

        [Fact]
        public async Task CreateRentalAsync_CarNotAvailable_ThrowsInvalidOperationException()
        {
            // Arrange
            _clientServiceMock.Setup(s => s.GetByIdAsync("client1")).ReturnsAsync(CreateClient());
            _carServiceMock.Setup(s => s.GetByIdAsync("car1")).ReturnsAsync(CreateCar(isAvailable: false));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreateRentalAsync("client1", "car1", DateTime.Today.AddDays(3), InsuranceType.Basic));
        }

        [Fact]
        public async Task CreateRentalAsync_TooYoungForPremium_ThrowsWithRequiredAge()
        {
            // Arrange — для Premium потрібно 25 років
            _clientServiceMock.Setup(s => s.GetByIdAsync("client1")).ReturnsAsync(CreateClient(age: 22));
            _carServiceMock.Setup(s => s.GetByIdAsync("car1")).ReturnsAsync(CreateCar(CarClass.Premium));

            // Act
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreateRentalAsync("client1", "car1", DateTime.Today.AddDays(3), InsuranceType.Basic));

            // Assert
            Assert.Contains("25", ex.Message);
        }

        [Fact]
        public async Task CreateRentalAsync_InsufficientExperience_ThrowsInvalidOperationException()
        {
            // Arrange — для Business потрібно 3 роки стажу
            _clientServiceMock.Setup(s => s.GetByIdAsync("client1")).ReturnsAsync(CreateClient(experience: 2));
            _carServiceMock.Setup(s => s.GetByIdAsync("car1")).ReturnsAsync(CreateCar(CarClass.Business));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreateRentalAsync("client1", "car1", DateTime.Today.AddDays(3), InsuranceType.Basic));
        }

        [Fact]
        public async Task CreateRentalAsync_InsufficientBalance_ThrowsAndDoesNotCreateContract()
        {
            // Arrange — застава 10000, а на балансі 5000
            _clientServiceMock.Setup(s => s.GetByIdAsync("client1")).ReturnsAsync(CreateClient(balance: 5000m));
            _carServiceMock.Setup(s => s.GetByIdAsync("car1")).ReturnsAsync(CreateCar(CarClass.Economy));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreateRentalAsync("client1", "car1", DateTime.Today.AddDays(3), InsuranceType.Basic));

            // Нічого не збережено в базу
            _contractRepoMock.Verify(r => r.CreateAsync(It.IsAny<CarContract>()), Times.Never);
            _clientServiceMock.Verify(s => s.UpdateAsync(It.IsAny<string>(), It.IsAny<Client>()), Times.Never);
        }

        // ReturnCarAsync — бізнес-задача 3 (повернення авто та застави)

        private CarContract SetupActiveRental(Client client, Car car)
        {
            var contract = new CarContract
            {
                Id = "contract1",
                ClientId = "client1",
                CarId = "car1",
                StartDate = DateTime.Today.AddDays(-2),   // 2 доби оренди → ліміт 700 км
                Status = ContractStatus.Active,
                DepositAmount = 10000m,
                StartMileage = 1000
            };
            _contractRepoMock.Setup(r => r.GetByIdAsync("contract1")).ReturnsAsync(contract);
            _clientServiceMock.Setup(s => s.GetByIdAsync("client1")).ReturnsAsync(client);
            _carServiceMock.Setup(s => s.GetByIdAsync("car1")).ReturnsAsync(car);
            return contract;
        }

        [Fact]
        public async Task ReturnCarAsync_ExtraMileage_ChargesAndRefundsRest()
        {
            // Arrange
            var client = CreateClient(balance: 0m);
            var car = CreateCar(isAvailable: false);
            SetupActiveRental(client, car);

            // Act — проїхали 800 км замість дозволених 700
            var result = await _service.ReturnCarAsync("contract1", endMileage: 1800);

            // Assert
            Assert.Equal(600m, result.ExtraMileageCharge);   // 100 км × 6 грн
            Assert.Equal(9400m, result.DepositRefund);       // 10000 − 600
            Assert.Equal(ContractStatus.Closed, result.Status);
            Assert.Equal(9400m, client.Balance);
            Assert.True(car.IsAvailable);
            Assert.Equal(1800, car.CurrentMileage);

            _contractRepoMock.Verify(r => r.ReplaceAsync("contract1",
                It.Is<CarContract>(c => c.Status == ContractStatus.Closed)), Times.Once);
            _carServiceMock.Verify(s => s.UpdateAsync("car1", It.Is<Car>(c => c.IsAvailable)), Times.Once);
        }

        [Fact]
        public async Task ReturnCarAsync_WithinMileageLimit_RefundsFullDeposit()
        {
            // Arrange
            var client = CreateClient(balance: 0m);
            SetupActiveRental(client, CreateCar(isAvailable: false));

            // Act — рівно 700 км (межа ліміту)
            var result = await _service.ReturnCarAsync("contract1", endMileage: 1700);

            // Assert
            Assert.Equal(0m, result.ExtraMileageCharge);
            Assert.Equal(10000m, result.DepositRefund);
        }

        [Fact]
        public async Task ReturnCarAsync_DamageExceedsDeposit_RefundIsZero()
        {
            // Arrange
            var client = CreateClient(balance: 0m);
            SetupActiveRental(client, CreateCar(isAvailable: false));

            // Act
            var result = await _service.ReturnCarAsync("contract1", endMileage: 1100, damageCost: 20000m);

            // Assert — повернення не може бути від'ємним
            Assert.Equal(0m, result.DepositRefund);
            Assert.Equal(0m, client.Balance);
        }

        [Fact]
        public async Task ReturnCarAsync_ContractNotFound_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.ReturnCarAsync("unknown", 2000));

            _contractRepoMock.Verify(r => r.ReplaceAsync(It.IsAny<string>(), It.IsAny<CarContract>()), Times.Never);
        }

        [Fact]
        public async Task ReturnCarAsync_ContractAlreadyClosed_ThrowsInvalidOperationException()
        {
            // Arrange
            var contract = SetupActiveRental(CreateClient(), CreateCar());
            contract.Status = ContractStatus.Closed;

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ReturnCarAsync("contract1", 2000));
        }

        [Fact]
        public async Task ReturnCarAsync_EndMileageLessThanStart_ThrowsArgumentException()
        {
            // Arrange
            SetupActiveRental(CreateClient(), CreateCar());

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.ReturnCarAsync("contract1", endMileage: 500));
        }
    }
}
