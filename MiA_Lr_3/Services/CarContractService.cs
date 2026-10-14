using System.Text.Json;
using MiA_Lr_3.Models;
using MiA_Lr_3.Models.Enum;
using MiA_Lr_3.Services.Interface;

namespace MiA_Lr_3.Services
{
    public class CarContractService : ICarContractService
    {
        private readonly string filePath = Path.Combine("Storage", "contracts.json");
        private readonly IClientService _clientService;
        private readonly ICarService _carService;

        public CarContractService(IClientService clientService, ICarService carService)
        {
            _clientService = clientService;
            _carService = carService;

            if (!File.Exists(filePath))
            {
                Directory.CreateDirectory("Storage");
                File.WriteAllText(filePath, "[]");
            }
        }

        private List<CarContract> ReadContracts()
        {
            string json = File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<List<CarContract>>(json) ?? new List<CarContract>();
        }

        private void WriteContracts(List<CarContract> contracts)
        {
            string json = JsonSerializer.Serialize(
                contracts,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(filePath, json);
        }

        // CRUD operations
        public List<CarContract> GetAll()
        {
            return ReadContracts();
        }

        public CarContract? GetById(int id)
        {
            return ReadContracts().FirstOrDefault(c => c.Id == id);
        }

        public CarContract Create(CarContract contract)
        {
            var contracts = ReadContracts();

            contract.Id = contracts.Count == 0 ? 1 : contracts.Max(c => c.Id) + 1;

            contracts.Add(contract);
            WriteContracts(contracts);

            return contract;
        }


        public bool Update(int id, CarContract contract)
        {
            var contracts = ReadContracts();

            int index = contracts.FindIndex(c => c.Id == id);

            if (index == -1)
                return false;

            contract.Id = id;
            contracts[index] = contract;

            WriteContracts(contracts);

            return true;
        }


        public bool Delete(int id)
        {
            var contracts = ReadContracts();

            var contract = contracts.FirstOrDefault(c => c.Id == id);

            if (contract == null)
                return false;

            contracts.Remove(contract);
            WriteContracts(contracts);

            return true;
        }
        //Бізнес задача 2
        public decimal CalculateRentalCost(decimal dailyRate, DateTime startDate, DateTime plannedEndDate, InsuranceType insurance)
        {
            int days = (plannedEndDate - startDate).Days;
            if (days < 1) {
                throw new ArgumentException("Мінімальний термін оренди — одна доба.");

            }
            decimal baseCost = days * dailyRate;
            decimal discount;

            if (days >= 30)
            {
                discount = 0.20m;
            }
            else if (days >= 7)
            {
                discount = 0.10m;
            }
            else
            {
                discount = 0m;
            }
            decimal totalCost = baseCost - (baseCost * discount);

            // Додаємо вартість розширеного страхування
            if (insurance == InsuranceType.FullCover)
            {
                totalCost += days * 400m;
            }
            else if (insurance != InsuranceType.Basic)
            {
                throw new ArgumentException("Невідомий тип страхування.");
            }

            // Повертаємо кінцеву вартість
            return totalCost;


        }

        //Бізнес задача 1
        public CarContract CreateRental(int clientId, int carId, DateTime plannedEndDate, InsuranceType insurance)
        {

            var client = _clientService.GetById(clientId);


            var car = _carService.GetById(carId);

            // Перевіряємо, чи існує клієнт
            if (client == null)
                throw new ArgumentException("Клієнта не знайдено.");

            // Перевіряємо, чи існує автомобіль
            if (car == null)
                throw new ArgumentException("Автомобіль не знайдено.");

            // Перевіряємо, чи доступний автомобіль
            if (!car.IsAvailable)
                throw new InvalidOperationException( "Автомобіль уже орендований.");
            // Перевіряємо, чи вказана дата народження
            if (client.BirthDate == null)
                throw new InvalidOperationException( "Не вказано дату народження клієнта.");

            // Обчислюємо вік клієнта
            DateTime today = DateTime.Today;
            DateTime birthDate = client.BirthDate.Value;

            int age = today.Year - birthDate.Year;

            // Перевіряємо, чи був день народження цього року
            if (birthDate.Date > today.AddYears(-age))
            {
                age--;
            }

            // Визначаємо вимоги залежно від класу автомобіля
            int requiredAge;
            int requiredExperience;

            switch (car.Class)
            {
                case CarClass.Economy:
                    requiredAge = 20;
                    requiredExperience = 1;
                    break;

                case CarClass.Business:
                case CarClass.SUV:
                    requiredAge = 23;
                    requiredExperience = 3;
                    break;

                case CarClass.Premium:
                    requiredAge = 25;
                    requiredExperience = 5;
                    break;

                default:
                    throw new InvalidOperationException( "Невідомий клас автомобіля.");
            }

            // Перевіряємо вік
            if (age < requiredAge)
                throw new InvalidOperationException( $"Для цього автомобіля потрібно мати щонайменше {requiredAge} років.");

            // Перевіряємо водійський стаж
            if (client.DrivingExperienceYears < requiredExperience)
                throw new InvalidOperationException($"Необхідний водійський стаж — щонайменше {requiredExperience} років.");
            // 1. Визначаємо суму застави (10 000 для Economy, 25 000 для інших)
            decimal depositAmount = car.Class == CarClass.Economy ? 10000m : 25000m;
            // 2. Перевіряємо баланс клієнта та блокуємо кошти
            if (client.Balance < depositAmount)
                throw new InvalidOperationException($"Недостатньо коштів для застави. Потрібно {depositAmount} грн (на балансі: {client.Balance} грн).");
            client.Balance -= depositAmount;
            _clientService.Update(client.Id, client);
            // 3. Рахуємо вартість оренди зі знижками та страховкою
            decimal totalCost = CalculateRentalCost(car.DailyRate, DateTime.Today, plannedEndDate, insurance);
            // 4. Створюємо договір зі статусом Active
            var contracts = ReadContracts();
            var newContract = new CarContract
            {
                Id = contracts.Count == 0 ? 1 : contracts.Max(c => c.Id) + 1,
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
            contracts.Add(newContract);
            WriteContracts(contracts);
            // 5. Позначаємо авто як тимчасово недоступне
            car.IsAvailable = false;
            _carService.Update(car.Id, car);
            return newContract;

        }
        // Бізнес-задача 3: Повернення автомобіля та врегулювання застави
        public CarContract ReturnCar(int contractId, int endMileage, decimal damageCost)
        {
            var contract = GetById(contractId);
            if (contract == null)
                throw new ArgumentException("Договір не знайдено.");
            if (contract.Status != ContractStatus.Active)
                throw new InvalidOperationException("Договір уже закритий або не є активним.");
            var car = _carService.GetById(contract.CarId);
            var client = _clientService.GetById(contract.ClientId);
            if (car == null || client == null)
                throw new InvalidOperationException("Помилка зв'язку: авто або клієнта не знайдено.");
            if (endMileage < contract.StartMileage)
                throw new ArgumentException("Кінцевий пробіг не може бути меншим за початковий.");
            // Розрахунок кількості діб оренди (мінімум 1)
            DateTime actualEnd = DateTime.Today;
            int rentalDays = Math.Max(1, (actualEnd - contract.StartDate).Days);
            // Ліміт 350 км/добу та перевірка перевищення (6 грн/км)
            int allowedMileage = rentalDays * 350;
            int actualMileage = endMileage - contract.StartMileage;
            decimal extraMileageCharge = 0m;
            if (actualMileage > allowedMileage)
            {
                extraMileageCharge = (actualMileage - allowedMileage) * 6m;
            }
            // Врегулювання застави
            decimal totalDeductions = damageCost + extraMileageCharge;
            decimal depositRefund = Math.Max(0m, contract.DepositAmount - totalDeductions);
            // Повертаємо залишок депозиту клієнту на баланс
            client.Balance += depositRefund;
            _clientService.Update(client.Id, client);
            // Оновлюємо договір
            contract.ActualEndDate = actualEnd;
            contract.EndMileage = endMileage;
            contract.DamageCost = damageCost;
            contract.ExtraMileageCharge = extraMileageCharge;
            contract.DepositRefund = depositRefund;
            contract.Status = ContractStatus.Closed;
            Update(contract.Id, contract);
            // Оновлюємо автомобіль (новий пробіг та повернення у статус доступного)
            car.CurrentMileage = endMileage;
            car.IsAvailable = true;
            _carService.Update(car.Id, car);
            return contract;
        }





    }


    }



