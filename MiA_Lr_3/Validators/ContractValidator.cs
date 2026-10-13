
using FluentValidation;
using MiA_Lr_3.Models;

namespace MiA_Lr_3.Validators
{
    public class ContractValidator : AbstractValidator<CarContract>
    {
        public ContractValidator()
        {
            RuleFor(contract => contract.ClientId)
                .GreaterThan(0)
                .WithMessage("Вкажіть коректний ідентифікатор клієнта");

            RuleFor(contract => contract.CarId)
                .GreaterThan(0)
                .WithMessage("Вкажіть коректний ідентифікатор автомобіля");

            RuleFor(contract => contract.StartDate)
                .Must(date => date != DateTime.MinValue)
                .WithMessage("Вкажіть дату початку оренди");

            RuleFor(contract => contract.PlannedEndDate)
                .Must((contract, endDate) =>
                    endDate != DateTime.MinValue &&
                    endDate >= contract.StartDate)
                .WithMessage("Дата завершення оренди має бути не раніше дати початку");

            RuleFor(contract => contract.Insurance)
                .IsInEnum()
                .WithMessage("Вкажіть коректний тип страхування");

            RuleFor(contract => contract.Status)
                .IsInEnum()
                .WithMessage("Вкажіть коректний статус договору");

            RuleFor(contract => contract.DepositAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Сума застави не може бути від'ємною");

            RuleFor(contract => contract.TotalCost)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Вартість оренди не може бути від'ємною");

            RuleFor(contract => contract.StartMileage)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Початковий пробіг не може бути від'ємним");

            RuleFor(contract => contract.EndMileage)
                .GreaterThanOrEqualTo(0)
                .When(contract => contract.EndMileage.HasValue)
                .WithMessage("Кінцевий пробіг не може бути від'ємним");

            RuleFor(contract => contract.ActualEndDate)
                .GreaterThanOrEqualTo(contract => contract.StartDate)
                .When(contract => contract.ActualEndDate.HasValue)
                .WithMessage("Фактична дата завершення не може бути раніше початку оренди");

            RuleFor(contract => contract.DamageCost)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Вартість пошкоджень не може бути від'ємною");

            RuleFor(contract => contract.ExtraMileageCharge)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Доплата за перевищення пробігу не може бути від'ємною");

            RuleFor(contract => contract.DepositRefund)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Сума повернення застави не може бути від'ємною");
        }
    }
}
