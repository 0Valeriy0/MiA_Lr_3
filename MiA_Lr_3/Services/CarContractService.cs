using MiA_Lr_3.Models;
using MiA_Lr_3.Models.Enum;
using MiA_Lr_3.Repositories.Interface;
using MiA_Lr_3.Services.Interface;

namespace MiA_Lr_3.Services
{
    public class CarContractService : ICarContractService
    {
        private readonly ICarContractRepository _contractRepository;
        private readonly IClientService _clientService;
        private readonly ICarService _carService;

        public CarContractService(
            ICarContractRepository contractRepository,
            IClientService clientService,
            ICarService carService)
        {
            _contractRepository = contractRepository;
            _clientService = clientService;
            _carService = carService;
        }

        // CRUD операції через репозиторій
        public async Task<IReadOnlyList<CarContract>> GetAllAsync() =>
            await _contractRepository.GetAllAsync();

        public async Task<CarContract?> GetByIdAsync(string id) =>
            await _contractRepository.GetByIdAsync(id);

        public async Task<CarContract> CreateAsync(CarContract contract) =>
            await _contractRepository.CreateAsync(contract);

        public async Task<bool> UpdateAsync(string id, CarContract contract) =>
            await _contractRepository.ReplaceAsync(id, contract);

        public async Task<bool> DeleteAsync(string id) =>
            await _contractRepository.DeleteAsync(id);

        // Бізнес-задача 2: Розрахунок вартості оренди
        public decimal CalculateRentalCost(decimal dailyRate, DateTime startDate, DateTime plannedEndDate, InsuranceType insurance)
        {
            int days = (plannedEndDate - startDate).Days;
            if (days < 1)
            {
                throw new ArgumentException("Мінімальний термін оренди — одна доба.");
            }

            decimal baseCost = days * dailyRate;
            decimal discount = 0m;

            if (days >= 30)
                discount = 0.20m;
            else if (days >= 7)
                discount = 0.10m;

            decimal totalCost = baseCost - (baseCost * discount);

            if (insurance == InsuranceType.FullCover)
            {
                totalCost += days * 400m;
            }
            else if (insurance != InsuranceType.Basic)
            {
                throw new ArgumentException("Невідомий тип страхування.");
            }

            return totalCost;
        }

        // Бізнес-задача 1: Оренда авто та перевірка цензу
        public async Task<CarContract> CreateRentalAsync(string clientId, string carId, DateTime plannedEndDate, InsuranceType insurance)
        {
            var client = await _clientService.GetByIdAsync(clientId);
            var car = await _carService.GetByIdAsync(carId);

            if (client == null)
                throw new ArgumentException("Клієнта не знайдено.");

            if (car == null)
                throw new ArgumentException("Автомобіль не знайдено.");

            if (!car.IsAvailable)
                throw new InvalidOperationException("Автомобіль уже орендований.");

            if (client.BirthDate == null)
                throw new InvalidOperationException("Не вказано дату народження клієнта.");

            DateTime today = DateTime.Today;
            DateTime birthDate = client.BirthDate.Value;
            int age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age))
                age--;

            int requiredAge = car.Class switch
            {
                CarClass.Economy => 20,
                CarClass.Business or CarClass.SUV => 23,
                CarClass.Premium => 25,
                _ => throw new InvalidOperationException("Невідомий клас автомобіля.")
            };

            int requiredExperience = car.Class switch
            {
                CarClass.Economy => 1,
                CarClass.Business or CarClass.SUV => 3,
                CarClass.Premium => 5,
                _ => throw new InvalidOperationException("Невідомий клас автомобіля.")
            };

            if (age < requiredAge)
                throw new InvalidOperationException($"Для цього автомобіля потрібно мати щонайменше {requiredAge} років.");

            if (client.DrivingExperienceYears < requiredExperience)
                throw new InvalidOperationException($"Необхідний водійський стаж — щонайменше {requiredExperience} років.");

            decimal depositAmount = car.Class == CarClass.Economy ? 10000m : 25000m;

            if (client.Balance < depositAmount)
                throw new InvalidOperationException($"Недостатньо коштів для застави. Потрібно {depositAmount} грн (на балансі: {client.Balance} грн).");

            // Блокуємо заставу на балансі клієнта
            client.Balance -= depositAmount;
            await _clientService.UpdateAsync(client.Id, client);

            // Розрахунок вартості
            decimal totalCost = CalculateRentalCost(car.DailyRate, DateTime.Today, plannedEndDate, insurance);

            // Створюємо договір у базі (Id згенерує MongoDB)
            var newContract = new CarContract
            {
                ClientId = clientId,
                CarId = carId,
                StartDate = DateTime.Today,
                PlannedEndDate = plannedEndDate,
                Insurance = insurance,
                Status = ContractStatus.Active,
                DepositAmount = depositAmount,
                TotalCost = totalCost,
                StartMileage = car.CurrentMileage
            };

            var createdContract = await _contractRepository.CreateAsync(newContract);

            // Авто стає недоступним
            car.IsAvailable = false;
            await _carService.UpdateAsync(car.Id, car);

            return createdContract;
        }

        // Бізнес-задача 3: Повернення автомобіля та врегулювання застави
        public async Task<CarContract> ReturnCarAsync(string contractId, int endMileage, decimal damageCost = 0)
        {
            var contract = await GetByIdAsync(contractId);
            if (contract == null)
                throw new ArgumentException("Договір не знайдено.");

            if (contract.Status != ContractStatus.Active)
                throw new InvalidOperationException("Договір уже закритий або не є активним.");

            var car = await _carService.GetByIdAsync(contract.CarId);
            var client = await _clientService.GetByIdAsync(contract.ClientId);

            if (car == null || client == null)
                throw new InvalidOperationException("Помилка зв'язку: авто або клієнта не знайдено.");

            if (endMileage < contract.StartMileage)
                throw new ArgumentException("Кінцевий пробіг не може бути меншим за початковий.");

            DateTime actualEnd = DateTime.Today;
            int rentalDays = Math.Max(1, (actualEnd - contract.StartDate).Days);

            int allowedMileage = rentalDays * 350;
            int actualMileage = endMileage - contract.StartMileage;
            decimal extraMileageCharge = 0m;

            if (actualMileage > allowedMileage)
            {
                extraMileageCharge = (actualMileage - allowedMileage) * 6m;
            }

            decimal totalDeductions = damageCost + extraMileageCharge;
            decimal depositRefund = Math.Max(0m, contract.DepositAmount - totalDeductions);

            // Повернення коштів на баланс клієнта
            client.Balance += depositRefund;
            await _clientService.UpdateAsync(client.Id, client);

            // Оновлюємо договір
            contract.ActualEndDate = actualEnd;
            contract.EndMileage = endMileage;
            contract.DamageCost = damageCost;
            contract.ExtraMileageCharge = extraMileageCharge;
            contract.DepositRefund = depositRefund;
            contract.Status = ContractStatus.Closed;
            await UpdateAsync(contract.Id, contract);

            // Автомобіль повертається в доступні
            car.CurrentMileage = endMileage;
            car.IsAvailable = true;
            await _carService.UpdateAsync(car.Id, car);

            return contract;
        }
    }
}