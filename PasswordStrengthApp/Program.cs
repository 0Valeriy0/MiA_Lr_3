using System.Text;
using PasswordStrengthApp.Services;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var service = new PasswordStrengthService();

Console.Write("Введіть пароль: ");
string input = service.PrepareInput(Console.ReadLine() ?? "");

Console.WriteLine($"Бал: {service.GetStrengthScore(input)}/{PasswordStrengthService.MaxScore}");
Console.WriteLine($"Рівень: {service.GetStrengthLevel(input)}");
Console.WriteLine(service.GetFirstMissingRequirement(input) ?? "Пароль відповідає всім вимогам");
